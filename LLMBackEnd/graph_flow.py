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
    class Config:
        extra = "allow"

# LLM 초기화
llm = ChatOpenAI(
    model="gpt-3.5-turbo",
    temperature=0.8,
    openai_api_key=os.getenv("OPENAI_API_KEY")
)

# NPC 응답 프롬프트 불러오기
template = PromptTemplate.from_template(
    open("prompts/npc_response_template.txt", encoding="utf-8").read()
)

# stress 판별 노드
def stress_node(state: GameState) -> GameState:
    npc = state.npc
    question = state.input

    if is_sensitive_question(question):
        npc_state.add_stress(npc, 2)
    else:
        npc_state.add_stress(npc, 0)
        
    return state.dict()

# 응답 생성 노드
def answer_node(state: GameState) -> GameState:
    npc = state.npc
    question = state.input

    print(f"NPC 이름: {npc}")
    print("현재 상태:", state.dict())

    stress = npc_state.get_stress(npc)
    history = npc_state.get_history(npc)
    history_text = "\n".join(history[-5:])
    stress_info = f"현재 스트레스 수준은 {stress}입니다. 스트레스가 높을수록 방어적이거나 혼란스러운 말을 할 수 있습니다."


    with open("setup.json", "r", encoding="utf-8") as f:
        data = json.load(f)
        suspects = data.get("suspects", [])
        target_npc = next((s for s in suspects if s["name"] == npc), None)

    if target_npc is None:
        raise Exception(f"❌ NPC '{npc}' not found in setup.json")

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
    npc_state.add_history(npc, f"플레이어: {question}\n{npc}: {response}")
    state.response = response.strip()

    return state.dict()

# sanitize 프롬프트
sanitize_prompt = PromptTemplate.from_template(
    open("prompts/sanitize_template.txt", encoding="utf-8").read()
)

# sanitize 노드
def sanitize_node(state: GameState) -> dict:
    response_text = state.response
    if not response_text or response_text.strip() == "":
        state.response = "상대방은 아무 말도 하지 않았다."
        return state.dict()

    prompt = sanitize_prompt.format(response=response_text)
    cleaned = llm.invoke(prompt).content.strip()
    state.response = cleaned or "상대방은 아무 말도 하지 않았다."
    return state.dict()

# 그래프 빌더 함수
def game_app():
    builder = StateGraph(schema=GameState)
    builder.add_node("stress_check", stress_node)
    builder.add_node("respond", answer_node)
    builder.add_node("sanitize", sanitize_node)

    builder.set_entry_point("stress_check")
    builder.add_edge("stress_check", "respond")
    builder.add_edge("respond", "sanitize")
    builder.set_finish_point("sanitize")

    return builder.compile()
