import os
import json
import uuid
from typing import Optional, List
from pydantic import BaseModel
from langchain_openai import ChatOpenAI
from langchain_core.prompts import PromptTemplate
from langgraph.graph import StateGraph
import chromadb
from chromadb.config import Settings

# .env 로드
from dotenv import load_dotenv
load_dotenv()

# LLM 설정
llm = ChatOpenAI(
    model="gpt-3.5-turbo",
    temperature=0.7,
    openai_api_key=os.getenv("OPENAI_API_KEY")
)

# ChromaDB
client = chromadb.Client(Settings(anonymized_telemetry=False))
collection = client.get_or_create_collection(name="memory")

# 상태 관리 모듈
from npc_agents.npc_memory import npc_state
from npc_agents.create_agents import BEHAVIOR_DESC, EMOTION_DESC

# 증거 ID → 이름 매핑
with open("evidence.json", "r", encoding="utf-8") as f:
    evidence_list = json.load(f)
evidence_map = {item["id"]: item["name"] for item in evidence_list}

# ───── 상태 정의 ─────
class GameState(BaseModel):
    npc: str
    evidence: dict  # {"id": "E_8"} 만 들어옴
    response: Optional[str] = None
    memory_used: Optional[List[str]] = []

# ───── 프롬프트 ─────
evidence_prompt = PromptTemplate.from_template(
    open("prompts/evidence_response_prompt.txt", encoding="utf-8").read()
)

# ───── Node 1: 기억 검색 ─────
def evidence_memory_search_node(state: GameState) -> GameState:
    npc = state.npc
    clue_id = state.evidence["id"]
    try:
        # ✅ 단일 where 조건만 줘야 함
        results = collection.query(
            query_texts=[clue_id],
            n_results=3,
            where={"npc": {"$eq": npc}}
        )
        raw_docs = results.get("documents", [[]])[0]
        filtered = list({doc for doc in raw_docs if clue_id in doc})
        state.memory_used = filtered
        print(f"[memory_search] {npc} 관련 기억: {filtered}")
    except Exception as e:
        print(f"[memory_search] 오류: {e}")
        state.memory_used = []
    return state.dict()

def evidence_response_node(state: GameState) -> GameState:
    npc = state.npc
    clue = state.evidence
    clue_id = clue["id"]

    # 🔧 안전하게 name 확보 (없으면 evidence_map에서 조회)
    clue_name = clue.get("name") or evidence_map.get(clue_id)
    if not clue_name:
        raise ValueError(f"[evidence_response_node] evidence_map에 {clue_id}에 해당하는 name이 없습니다.")

    clue_importance = clue.get("importance", "미상")
    memory_text = "\n".join(state.memory_used or [])

    # NPC 정보 로드
    with open("setup.json", "r", encoding="utf-8") as f:
        suspects = json.load(f)["suspects"]
    target = next((s for s in suspects if s["name"] == npc), None)
    if not target:
        raise Exception(f"{npc} not found in setup.json")

    behavior = target["personality"]["behavior"]
    emotion = target["personality"]["emotion"]
    occupation = target["occupation"]
    statement = target["statement"]
    is_witch = target["is_witch"]

    stress = npc_state.get_stress(npc)
    anxiety = npc_state.get_anxiety(npc)

    prompt = evidence_prompt.format(
        name=npc,
        behavior=behavior,
        emotion=emotion,
        occupation=occupation,
        statement=statement,
        truth_or_lie="lying" if is_witch else "truthful",
        truth_or_lie_detail="You are the witch and must lie." if is_witch else "You are innocent and may speak honestly.",
        behavior_desc=BEHAVIOR_DESC[behavior],
        emotion_desc=EMOTION_DESC[emotion],
        stress=stress,
        anxiety=anxiety,
        memory_text=memory_text,
        evidence_id=clue_id,
        evidence_name=clue_name,
        evidence_importance=clue_importance,
    )

    response = llm.invoke(prompt).content.strip()
    state.response = response
    npc_state.add_history(npc, f"[증거 제출] {clue_name}:\n{response}")
    print(f"[evidence_response] {npc} 응답:\n{response}")
    return state.dict()

# ───── Node 3: 저장 ─────
def store_memory_node(state: GameState) -> GameState:
    npc = state.npc
    content = state.response
    if content:
        collection.add(
            documents=[f"{npc}: {content}"],
            metadatas=[{"npc": npc}],
            ids=[str(uuid.uuid4())]
        )
    return state.dict()

# ───── 그래프 정의 ─────
def evidence_graph():
    builder = StateGraph(schema=GameState)
    builder.add_node("evidence_memory_search", evidence_memory_search_node)
    builder.add_node("evidence_response", evidence_response_node)
    builder.add_node("store_memory", store_memory_node)

    builder.set_entry_point("evidence_memory_search")
    builder.add_edge("evidence_memory_search", "evidence_response")
    builder.add_edge("evidence_response", "store_memory")
    builder.set_finish_point("store_memory")

    return builder.compile()
