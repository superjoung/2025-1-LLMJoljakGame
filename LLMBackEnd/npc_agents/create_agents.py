import os
import json
from dotenv import load_dotenv
from typing import TypedDict
from langchain.prompts import PromptTemplate
from langchain.chat_models import ChatOpenAI
from langgraph.graph import StateGraph

load_dotenv()

llm = ChatOpenAI(
    model="gpt-3.5-turbo",
    temperature=0.9,
    openai_api_key=os.getenv("OPENAI_API_KEY")
)

# 상태 저장용 타입
class GameState(TypedDict):
    question: str
    answer: str

# 성격 설명
BEHAVIOR_DESC = {
    "extroverted": "sociable and outgoing",
    "introverted": "quiet and reserved"
}
EMOTION_DESC = {
     "timid": "Easily frightened and speaks hesitantly, often avoiding direct eye contact.",
    "suspicious": "Distrustful and defensive, often implying hidden motives in others.",
    "cheerful": "Lighthearted, friendly, and speaks with a warm, upbeat tone.",
    "devout": "Deeply religious, often references faith or divine justice when speaking.",
    "talkative": "Speaks at length and without pause, often digressing or overexplaining.",
    "indifferent": "Uninterested and flat in tone, responds briefly or lazily.",
    "hot-tempered": "Easily angered, speaks in sharp or frustrated tones.",
    "secretive": "Carefully avoids details, speaks cautiously and with vague phrases.",
    "depressed": "Low energy and gloomy, often pessimistic or resigned in tone.",
    "dull-minded": "Due to lack of intelligence, they have difficulty understanding speech or situations. Slow to respond, uses simple words or confused logic.",
    "gullible": "Easily influenced, repeats rumors or believes questionable ideas.",
    "grumpy": "Annoyed by questions, dismissive, sighs or complains often.",
    "sleepy": "Speaks slowly and yawns often, loses focus or forgets what was asked.",
    "pessimistic": "Sees the worst in every situation, often fatalistic or hopeless."
}

# 성격 → 말투 맵
TONE_STYLE = {
    "timid": "존댓말",
    "suspicious": "반말",
    "cheerful": "반말",
    "devout": "존댓말",
    "talkative": "반말",
    "indifferent": "반말",
    "hot-tempered": "반말",
    "secretive": "존댓말",
    "depressed": "존댓말",
    "dull-minded": "반말",
    "gullible": "반말",
    "grumpy": "반말",
    "sleepy": "반말",
    "pessimistic": "존댓말"
}


def load_template(path: str) -> PromptTemplate:
    with open(path, "r", encoding="utf-8") as f:
        return PromptTemplate.from_template(f.read())

def create_npc_node(npc, template: PromptTemplate):
    behavior = npc["personality"]["behavior"]
    emotion = npc["personality"]["emotion"]
    is_witch = npc["is_witch"]

    prompt = template.format(
        name=npc["name"],
        behavior=behavior,
        emotion=emotion,
        occupation=npc["occupation"],
        statement=npc["statement"],
        truth_or_lie="lying" if is_witch else "truthful",
        truth_or_lie_detail="You are the witch and must lie." if is_witch else "You are innocent and telling the truth.",
        behavior_desc=BEHAVIOR_DESC[behavior],
        emotion_desc=EMOTION_DESC[emotion],
        tone_style=TONE_STYLE[emotion]
    )

    def node_func(state: GameState) -> GameState:
        user_input = state.get("question", "")
        answer = llm.predict(prompt + f"\n\nQuestion: {user_input}")
        return {"question": user_input, "answer": answer}

    return node_func

def build_npc_graph():
    with open("setup.json", "r", encoding="utf-8") as f:
        setup = json.load(f)

    suspects = setup["suspects"]
    template = load_template("prompts/npc_response_template.txt")

    builder = StateGraph(schema=GameState)
    
    for npc in suspects:
        node = create_npc_node(npc, template)
        builder.add_node(npc["name"], node)

    # ✅ 순차적으로 NPC 노드 연결
    for i in range(len(suspects) - 1):
        builder.add_edge(suspects[i]["name"], suspects[i + 1]["name"])

    # ✅ 마지막 노드 종료 지점 지정
    builder.add_node("end", lambda state: state)
    builder.add_edge(suspects[-1]["name"], "end")
    builder.set_finish_point("end")

    # ✅ 시작 노드 지정
    builder.set_entry_point(suspects[0]["name"])

    return builder.compile()


if __name__ == "__main__":
    graph = build_npc_graph()
    print("✅ LangGraph NPC Agent system successfully built!")
