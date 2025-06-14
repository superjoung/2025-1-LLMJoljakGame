import os
import json
import uuid
from typing import Optional
from pydantic import BaseModel
from langchain_core.prompts import PromptTemplate
from langchain_openai import ChatOpenAI
from langgraph.graph import StateGraph
from npc_agents.create_agents import BEHAVIOR_DESC, EMOTION_DESC
from npc_agents.create_agents import TONE_STYLE
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
# 일반 필터링/상태 체크용 (GPT-3.5)
llm = ChatOpenAI(  
    model="gpt-3.5-turbo",
    temperature=0.5,
    openai_api_key=os.getenv("OPENAI_API_KEY")
)

# 자연스러운 응답 전용 (GPT-4)
llm_respond = ChatOpenAI(  
    model="gpt-4",
    temperature=0.5,
    openai_api_key=os.getenv("OPENAI_API_KEY")
)

# --- 프롬프트 로딩 ---
filter_input_prompt = PromptTemplate.from_template(
    open("prompts/filter_input_prompt.txt", encoding="utf-8").read()
)
status_update_prompt = PromptTemplate.from_template(
    open("prompts/status_update_prompt.txt", encoding="utf-8").read()
)

template = PromptTemplate.from_template(
    open("prompts/npc_response_template.txt", encoding="utf-8").read()
)

# --- Node 1: Filter Input ---
def filter_input_node(state: GameState) -> GameState:
    print("[filter_input_node] 입력 필터링 시작:", state.input)
    prompt = filter_input_prompt.format(user_input=state.input)
    result = llm.invoke(prompt).content.strip()
    
    try:
        # 첫 번째 파싱
        result_json = json.loads(result)
        # 문자열 이중 파싱 방지
        if isinstance(result_json, str):
            result_json = json.loads(result_json)
        print("[filter_input_node] 응답 JSON:", result_json)
        # allowed 값을 문자열로 받을 경우 bool로 변환
        allowed_value = result_json.get("allowed", True)
        if isinstance(allowed_value, str):
            allowed_value = allowed_value.lower() == "true"
        state.allowed = allowed_value
        if not allowed_value:
            state.response = result_json.get("reason", "부적절한 입력입니다. 다시 질문해주세요.")
    except Exception as e:
        print(f"[filter_input_node] JSON 파싱 오류: {e}")
        state.response = f"입력 필터링 중 오류 발생: {e}"
        state.allowed = False

    return state.dict()

# --- Node 2: Status Check ---
def status_node(state: GameState) -> GameState:
    print("[status_node] 스테이터스 체크 시작")

    if not state.allowed:
        print("[status_node] 필터링 결과로 인해 건너뜀")
        return state.dict()
    npc = state.npc
    question = state.input

    # setup.json 기반 NPC 정보 불러오기
    with open("setup.json", "r", encoding="utf-8") as f:
        data = json.load(f)
        target_npc = next((n for n in data["suspects"] if n["name"] == npc), None)

    if not target_npc:
        print(f"[status_node] NPC '{npc}' 정보 없음")
        return state.dict()
    else:
        print(f"[status_node] NPC '{npc}' 정보 찾음")

    # 성격, 상태 정보
    is_witch = target_npc["is_witch"]
    behavior = target_npc["personality"]["behavior"]
    emotion = target_npc["personality"]["emotion"]
    stress = npc_state.get_stress(npc)
    anxiety = npc_state.get_anxiety(npc)

    # 프롬프트 채우기
    prompt = status_update_prompt.format(
        name=npc,
        is_witch=is_witch,
        behavior=behavior,
        emotion=emotion,
        stress=stress,
        anxiety=anxiety,
        question=question,
    )

    response = llm.invoke(prompt).content.strip()

    try:
        result = json.loads(response)
        stress_delta = result.get("stress", 0)
        anxiety_delta = result.get("anxiety", 0)

        npc_state.add_stress(npc, stress_delta)
        npc_state.add_anxiety(npc, anxiety_delta)

        # 최종 수치
        updated_stress = npc_state.get_stress(npc)
        updated_anxiety = npc_state.get_anxiety(npc)

        print(f"[status_node] '{npc}' 스트레스 +{stress_delta} → 현재: {updated_stress}")
        print(f"[status_node] '{npc}' 불안감 +{anxiety_delta} → 현재: {updated_anxiety}")
        print(f"[이유] {result.get('reason')}")

    except Exception as e:
        print(f"[status_node] JSON 파싱 실패: {e}\nLLM 응답:\n{response}")
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

# --- 요약 함수들 ---
# --- 관련된 기억 5개를 추려 그 중에 중요한 기억에 가중치를 부여하여 3개를 뽑는다. 그 후 요약하여 전달한다. ---
# 세계관, 과거 설정 및 기억들 
def summarize_memories(memories, npc_name=None, max_len=3):
    if not memories:
        return ""

    scored = []
    for m in memories:
        score = 0
        
        # 기억에 현재 NPC 이름이 포함되어 있다면 가중치 부여
        if npc_name and npc_name in m:
            score += 2

        # '사회자'라는 단어가 포함되어 있으면 추가 점수    
        if "사회자" in m:
            score += 1
        
        # 기억의 길이에 따라 정보량 점수 부여 (길수록 더 중요하다고 판단)
        score += len(m) / 50  # 정보량 점수
        scored.append((score, m))

    # 상위 3개를 요약
    sorted_memories = sorted(scored, key=lambda x: x[0], reverse=True)
    selected = [mem for _, mem in sorted_memories[:max_len]]
    return "\n".join(selected)

# --- 플레이어와 npc 간 대화 간단하게 요약 ---
def summarize_history(history_list, max_lines=2):
    if not history_list:
        return ""
    return " / ".join([line.split("\n")[-1] for line in history_list[-max_lines:]])


# --- Node 4: Answer Generation ---
def answer_node(state: GameState) -> GameState:
    if not state.allowed:
        return state.dict()

    npc = state.npc
    question = state.input
    stress = npc_state.get_stress(npc)
    anxiety = npc_state.get_anxiety(npc)
    history = npc_state.get_history(npc)

    # 요약된 memory, history 적용
    memory_text = summarize_memories(state.memory_used, npc_name=npc)
    history_text = summarize_history(history)

    with open("setup.json", "r", encoding="utf-8") as f:
        data = json.load(f)
        suspects = data.get("suspects", [])
        target_npc = next((s for s in suspects if s["name"] == npc), None)

    if target_npc is None:
        raise Exception(f"NPC '{npc}' not found in setup.json")

    behavior = target_npc["personality"]["behavior"]
    emotion = target_npc["personality"]["emotion"]
    occupation = target_npc["occupation"]
    statement = target_npc["statement"]
    is_witch = target_npc["is_witch"]
    gender = target_npc["gender"]
    age_group = target_npc["age_group"]


    truth_or_lie = "lying" if is_witch else "truthful"
    truth_or_lie_detail = "You are the witch and must lie." if is_witch else "You are innocent and telling the truth."

    prompt = template.format(
        name=npc,
        question=question,
        memory=memory_text,
        stress=stress,
        anxiety=anxiety,
        history=history_text,
        behavior=behavior,
        emotion=emotion,
        occupation=occupation,
        statement=statement,
        truth_or_lie=truth_or_lie,
        truth_or_lie_detail=truth_or_lie_detail,
        behavior_desc=BEHAVIOR_DESC[behavior],
        emotion_desc=EMOTION_DESC[emotion],
        gender=gender,
        age_group=age_group,
        tone_style=TONE_STYLE[emotion]
    )

    response = llm_respond.invoke(prompt)
    reply_text = response.content.strip()
    state.response = reply_text
    npc_state.add_history(npc, f"플레이어: {question}\n{npc}: {reply_text}")


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

    else:
        collection.add(
        documents=[f"{npc}: {content}"],
        metadatas=[{"npc": npc}],
        ids=[str(uuid.uuid4())]
    )
    return state.dict()

# --- LangGraph 빌더 ---
def response_graph():
    builder = StateGraph(schema=GameState)
    builder.add_node("filter_input", filter_input_node)
    builder.add_node("status_node", status_node)
    builder.add_node("memory_search", memory_search_node)
    builder.add_node("respond", answer_node)
    builder.add_node("store_memory", memory_store_node)

    builder.set_entry_point("filter_input")
    builder.add_edge("filter_input", "status_node")
    builder.add_edge("status_node", "memory_search")
    builder.add_edge("memory_search", "respond")
    builder.add_edge("respond", "store_memory")
    builder.set_finish_point("store_memory")

    return builder.compile()