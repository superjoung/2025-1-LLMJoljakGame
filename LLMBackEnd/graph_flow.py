# graph_flow.py
from langgraph.graph import StateGraph
from typing import TypedDict, List, Dict
from npc_agent import respond_as_npc

class GraphState(TypedDict):
    input: str
    npc: str
    chat_history: List[Dict[str, str]]

def build_npc_node(npc_name, personality, context):
    def node(state):
        player_input = state["input"]
        npc_reply = respond_as_npc(npc_name, personality, context, player_input)
        state["chat_history"].append({"npc": npc_name, "response": npc_reply})
        return state
    return node

graph = StateGraph(GraphState)  # ✅ 필수 인자 schema!
graph.add_node("지훈", build_npc_node("지훈", "겁많고 정직한 청년", "그날 마당에 있었다"))
graph.set_entry_point("지훈")
graph.set_finish_point("지훈")

game_app = graph
