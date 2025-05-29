import os
import json
import uuid
from dotenv import load_dotenv
from langchain_openai import ChatOpenAI
from langchain_core.prompts import PromptTemplate
import chromadb
from chromadb.config import Settings

# 환경 변수 로드
load_dotenv()

# LLM 설정
llm = ChatOpenAI(
    model="gpt-3.5-turbo",
    temperature=0.9,
    openai_api_key=os.getenv("OPENAI_API_KEY")
)

# 프롬프트 템플릿 로드
route_prompt = PromptTemplate.from_template(
    open("prompts/route_planner.txt", encoding="utf-8").read()
)

clue_prompt = PromptTemplate.from_template(
    open("prompts/clue_generate_prompt.txt", encoding="utf-8").read()
)

# ChromaDB 클라이언트 초기화
client = chromadb.Client(Settings(anonymized_telemetry=False))
collection = client.get_or_create_collection(name="memory")

def plan_full_game_data() -> dict:
    # 1. setup.json 읽기
    with open("setup.json", "r", encoding="utf-8") as file:
        data = json.load(file)

    npc_list = [
        {"name": suspect["name"], "behavior": suspect["personality"]["behavior"]}
        for suspect in data["suspects"]
    ]

    witch_name = next((sus["name"] for sus in data["suspects"] if sus.get("is_witch")), None)
    if not witch_name:
        raise ValueError("setup.json에 'is_witch': true 로 지정된 NPC가 없습니다.")

    # 2. 경로 생성 프롬프트
    route_input = route_prompt.format(
        name1=npc_list[0]["name"], behavior1=npc_list[0]["behavior"],
        name2=npc_list[1]["name"], behavior2=npc_list[1]["behavior"],
        name3=npc_list[2]["name"], behavior3=npc_list[2]["behavior"],
        name4=npc_list[3]["name"], behavior4=npc_list[3]["behavior"],
        name5=npc_list[4]["name"], behavior5=npc_list[4]["behavior"]
    )

    # 3. 루트 생성
    route_response = llm.invoke(route_input).content.strip()
    print("[LLM 응답 - 루트]")

    try:
        route_data = json.loads(route_response)
        npc_routes = route_data["npc_routes"]
        print(npc_routes)
    except json.JSONDecodeError:
        print("루트 JSON 파싱 실패:")
        print(route_response)
        return {}

     # 3-1. 루트 정보를 DB에 저장 (NPC별 방문 장소)
    for npc_name, locations in npc_routes.items():
        for idx, location in enumerate(locations):
            memory_text = f"나는 {idx+1}번째에 {location}에 갔다."
            collection.add(
                documents=[f"{npc_name}: {memory_text}"],
                metadatas=[
                    {
                        "npc": npc_name,
                        "type": "route",
                        "order": idx + 1,
                        "location": location
                    }
                ],
                ids=[str(uuid.uuid4())]
            )
            print(f"[Memory 저장 - 경로] {npc_name}: {memory_text}"
            
    # 4. 증거 생성
    clue_input = clue_prompt.format(
        witch_name=witch_name,
        routes_json=json.dumps({"npc_routes": npc_routes}, ensure_ascii=False, indent=2)
    )
    clue_response = llm.invoke(clue_input).content.strip()
    print("[LLM 응답 - 증거]")
    print(clue_response)

    try:
        clues_data = json.loads(clue_response)
        clues = clues_data.get("clues", [])
    except json.JSONDecodeError as e:
        print("증거 JSON 파싱 실패:", e)
        print("원본 응답:\n", clue_response)
        clues = []

    # 4-1. 마녀 NPC의 memory DB에 증거 관련 기억 저장
    for clue in clues:
        memory_text = f"나는 {clue['location']}에 {clue['name']}({clue['id']})을(를) 두었다."
        collection.add(
            documents=[f"{witch_name}: {memory_text}"],
            metadatas=[
                {
                    "npc": witch_name,
                    "type": "evidence",
                    "evidence_id": clue["id"]
                }
            ],
            ids=[str(uuid.uuid4())]
        )
        print(f"[Memory 저장] {witch_name}: {memory_text}")

    # 5. 반환
    return {
        "npc_routes": npc_routes,
        "clues": clues
    }
