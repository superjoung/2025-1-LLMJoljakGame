import os
import json
import uuid
from typing import Optional
from pydantic import BaseModel
from langchain_core.prompts import PromptTemplate
from langchain_openai import ChatOpenAI
from langgraph.graph import StateGraph
from npc_agents.create_agents import BEHAVIOR_DESC, EMOTION_DESC
from npc_agents.stress_judge import is_sensitive_question
from npc_agents.npc_memory import npc_state
import chromadb
from chromadb.config import Settings

# ✅ ChromaDB 설정
client = chromadb.Client(Settings(anonymized_telemetry=False))
collection = client.get_or_create_collection(name="memory")

# ✅ setup.json에 있는 사회자 기억을 memory에 등록 (한 번만 수행)
with open("setup.json", "r", encoding="utf-8") as f:
    data = json.load(f)
    village_desc = data.get("setup", {}).get("village", "")
    event_desc = data.get("setup", {}).get("event", "")

    if village_desc:
        collection.add(
            documents=[f"사회자: {village_desc}"],
            metadatas=[{"npc": "사회자"}],
            ids=["world_village"]
        )
    if event_desc:
        collection.add(
            documents=[f"사회자: {event_desc}"],
            metadatas=[{"npc": "사회자"}],
            ids=["world_event"]
        )

# 상태 클래스 정의
class GameState(BaseModel):
    npc: str
    input: str
    response: Optional[str] = None
    allowed: bool = True
    memory_used: Optional[list] = []

# LLM 초기화
llm = ChatOpenAI(
    model="gpt-3.5-turbo",
    temperature=0.8,
    openai_api_key=os.getenv("OPENAI_API_KEY")
)

# --- 프롬프트 로딩 ---
filter_input_prompt = PromptTemplate.from_template(
    open("prompts/filter_input_prompt.txt", encoding="utf-8").read()
)
template = PromptTemplate.from_template(
    open("prompts/npc_response_template.txt", encoding="utf-8").read()
)

# --- Node 1: Filter Input ---
def filter_input_node(state: GameState) -> GameState:
    prompt = filter_input_prompt.format(user_input=state.input)
    try:
        result_json = json.loads(llm.invoke(prompt).content.strip())
        if isinstance(result_json, str):
            result_json = json.loads(result_json)
        state.allowed = result_json.get("allowed", True)
        if not state.allowed:
            state.response = result_json.get("reason", "부적절한 입력입니다.")
    except:
        state.allowed = False
        state.response = "입력 분석 오류"
    return state.dict()

# --- Node 2: Stress check ---
def stress_node(state: GameState) -> GameState:
    if not state.allowed:
        return state.dict()
    npc = state.npc
    question = state.input

    if is_sensitive_question(question):
        npc_state.add_stress(npc, 2)
        print(f"[stress_node] 민감한 질문 감지: '{npc}' 스트레스 +2")
    else:
        print(f"[stress_node] 민감하지 않음: '{npc}' 스트레스 변화 없음")

    return state.dict()

# --- Node 3: Memory Searching ---
def memory_search_node(state: GameState) -> GameState:
    if not state.allowed:
        return state.dict()

    npc = state.npc
    query = f"{npc}의 기억과 관련된 질문 : {state.input}"

    try:
        results = collection.query(
            query_texts=[query],
            n_results=5,
            where={"npc": {"$in": [npc, "사회자"]}}
        )

        raw_docs = results.get("documents", [[]])[0]
        filtered_memory = list({
            doc for doc in raw_docs
            if not any(ban in doc for ban in ["이 설정에는", "허용되지 않습니다", "일치하지 않습니다"])
        })

        state.memory_used = filtered_memory
        print(f"[memory] 검색된 기억: {filtered_memory}")

    except Exception as e:
        print(f"[memory] 기억 검색 오류: {e}")
        state.memory_used = []

    return state.dict()

# --- Node 4: Answer Generation ---
def answer_node(state: GameState) -> GameState:
    if not state.allowed:
        return state.dict()

    npc = state.npc
    question = state.input
    stress = npc_state.get_stress(npc)
    history = npc_state.get_history(npc)
    memory_text = "\n".join(state.memory_used or [])
    history_text = "\n".join(history[-5:])
    stress_info = f"현재 스트레스 수준은 {stress}입니다. 스트레스가 높을수록 방어적이거나 혼란스러운 말을 할 수 있습니다."

    with open("setup.json", "r", encoding="utf-8") as f:
        data = json.load(f)
        suspects = data.get("suspects", [])
        target_npc = next((s for s in suspects if s["name"] == npc), None)

    if not target_npc:
        return state.dict()

    prompt = template.format(
        name=npc,
        question=question,
        memory=memory_text,
        stress=stress,
        history=history_text,
        stress_info=stress_info,
        behavior=target_npc["personality"]["behavior"],
        emotion=target_npc["personality"]["emotion"],
        occupation=target_npc["occupation"],
        statement=target_npc["statement"],
        truth_or_lie="lying" if target_npc["is_witch"] else "truthful",
        truth_or_lie_detail="You are the witch and must lie." if target_npc["is_witch"] else "You are innocent and telling the truth.",
        behavior_desc=BEHAVIOR_DESC[target_npc["personality"]["behavior"]],
        emotion_desc=EMOTION_DESC[target_npc["personality"]["emotion"]],
    )

    response = llm.invoke(prompt).content or ""
    npc_state.add_history(npc, f"플레이어: {question}\n{npc}: {response}")
    state.response = response.strip()
    return state.dict()

# --- Node 5: Answer Storation --- 
def memory_store_node(state: GameState) -> GameState:
    npc = state.npc
    content = state.input if npc == "사회자" else state.response

     # 설정 위반 메시지일 경우 저장하지 않음
    ban_phrases = ["이 설정에는", "허용되지 않습니다", "일치하지 않습니다"]
    if any(phrase in content for phrase in ban_phrases):
        print(f"[memory_store] 저장 제외됨: '{content}'")
        return state.dict()

    collection.add(
        documents=[f"{npc}: {content}"],
        metadatas=[{"npc": npc}],
        ids=[str(uuid.uuid4())]
    )
    return state.dict()

# --- LangGraph 빌더 ---
def game_app():
    builder = StateGraph(schema=GameState)
    builder.add_node("filter_input", filter_input_node)
    builder.add_node("stress_check", stress_node)
    builder.add_node("memory_search", memory_search_node)
    builder.add_node("respond", answer_node)
    builder.add_node("store_memory", memory_store_node)

    builder.set_entry_point("filter_input")
    builder.add_edge("filter_input", "stress_check")
    builder.add_edge("stress_check", "memory_search")
    builder.add_edge("memory_search", "respond")
    builder.add_edge("respond", "store_memory")
    builder.set_finish_point("store_memory")

    return builder.compile()
