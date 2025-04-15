# graph_flow.py

from langgraph.graph import StateGraph
from typing import TypedDict, List, Dict
from npc_agent import respond_as_npc  # GPT 응답 함수

# 🔹 그래프에 전달될 상태 구조 정의
class GraphState(TypedDict):
    input: str
    npc: str
    chat_history: List[Dict[str, str]]

# 🔹 NPC별 노드 생성 함수
def build_npc_node(npc_name: str, personality: str, context: str):
    def node(state: GraphState) -> GraphState:
        player_input = state["input"]
        npc_reply = respond_as_npc(npc_name, personality, context, player_input)
        state["chat_history"].append({"npc": npc_name, "response": npc_reply})
        return state
    return node

# ✅ 요청된 npc에 따라 그래프 동적 생성
def create_dynamic_graph(npc_name: str):
    graph = StateGraph(GraphState)

    if npc_name == "사회자":
        # 사회자는 GPT 호출 없이 저장만 → 빈 노드 생성
        def host_node(state: GraphState) -> GraphState:
            state["chat_history"].append({"npc": "사회자", "response": state["input"]})
            return state
        graph.add_node("사회자", host_node)

    elif npc_name == "지훈":
        graph.add_node("지훈", build_npc_node("지훈", "겁많고 정직한 청년", "그날 마당에 있었다"))

    else:
        # 기본형 NPC 처리
        graph.add_node(npc_name, build_npc_node(npc_name, "기본 성격", "기본 배경"))

    graph.set_entry_point(npc_name)
    graph.set_finish_point(npc_name)
    return graph.compile()
