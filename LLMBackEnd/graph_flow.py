import os
import json
from typing import Optional
from pydantic import BaseModel
from langchain_core.prompts import PromptTemplate
from langchain_openai import ChatOpenAI
from langgraph.graph import StateGraph
from npc_agents.create_agents import BEHAVIOR_DESC, EMOTION_DESC
from npc_agents.stress_judge import is_sensitive_question
from npc_agents.npc_memory import npc_state

# 상태 클래스 정의
class GameState(BaseModel):
    npc: str
    input: str
    response: Optional[str] = None
    allowed: bool = True

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


# --- Node 2: Stress Check ---
def stress_node(state: GameState) -> GameState:
    print("[stress_node] 스트레스 체크 시작")

    if not state.allowed:
        print("[stress_node] 필터링 결과로 인해 건너뜀")
        return state.dict()

    npc = state.npc
    question = state.input

    if is_sensitive_question(question):
        npc_state.add_stress(npc, 2)
        print(f"[stress_node] 민감한 질문 감지. '{npc}'의 스트레스 +2")
    else:
        npc_state.add_stress(npc, 0)
        print(f"[stress_node] 민감하지 않음. '{npc}'의 스트레스 변화 없음")

    return state.dict()

# --- Node 3: Answer Generation ---
def answer_node(state: GameState) -> GameState:
    print("[answer_node] NPC 응답 생성 시작")

    if not state.allowed:
        print("[answer_node] 필터링 결과로 인해 건너뜀")
        return state.dict()

    npc = state.npc
    question = state.input
    stress = npc_state.get_stress(npc)
    history = npc_state.get_history(npc)
    history_text = "\n".join(history[-5:])
    stress_info = f"현재 스트레스 수준은 {stress}입니다. 스트레스가 높을수록 방어적이거나 혼란스러운 말을 할 수 있습니다."

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

    truth_or_lie = "lying" if is_witch else "truthful"
    truth_or_lie_detail = "You are the witch and must lie." if is_witch else "You are innocent and telling the truth."

    prompt = template.format(
        name=npc,
        question=question,
        stress=stress,
        history=history_text,
        stress_info=stress_info,
        behavior=behavior,
        emotion=emotion,
        occupation=occupation,
        statement=statement,
        truth_or_lie=truth_or_lie,
        truth_or_lie_detail=truth_or_lie_detail,
        behavior_desc=BEHAVIOR_DESC[behavior],
        emotion_desc=EMOTION_DESC[emotion],
    )

    response = llm.invoke(prompt).content or ""
    print(f"[answer_node] '{npc}' 응답:\n{response}")
    npc_state.add_history(npc, f"플레이어: {question}\n{npc}: {response}")
    state.response = response.strip()
    return state.dict()

# --- LangGraph 빌더 ---
def game_app():
    builder = StateGraph(schema=GameState)
    builder.add_node("filter_input", filter_input_node)
    builder.add_node("stress_check", stress_node)
    builder.add_node("respond", answer_node)

    builder.set_entry_point("filter_input")
    builder.add_edge("filter_input", "stress_check")
    builder.add_edge("stress_check", "respond")
    builder.set_finish_point("respond")

    return builder.compile()
