import os
import json
from dotenv import load_dotenv
from langchain_openai import ChatOpenAI
from langchain_core.prompts import PromptTemplate
from typing import Dict, List

load_dotenv()

llm = ChatOpenAI(
    model="gpt-3.5-turbo",
    temperature=0.9,
    openai_api_key=os.getenv("OPENAI_API_KEY")
)

filter_input_prompt = PromptTemplate.from_template(
    open("prompts/route_planner.txt", encoding="utf-8").read()
)

def plan_route() -> dict:
    # setup.json 파일 열기
    with open("setup.json", "r", encoding="utf-8") as file:
        data = json.load(file)

    npc_list = [
        {"name": suspect["name"], "behavior": suspect["personality"]["behavior"]}
        for suspect in data["suspects"]
    ]

    # 각 NPC 이름 및 성격 추출
    prompt = filter_input_prompt.format(
        name1=npc_list[0]["name"], behavior1=npc_list[0]["behavior"],
        name2=npc_list[1]["name"], behavior2=npc_list[1]["behavior"],
        name3=npc_list[2]["name"], behavior3=npc_list[2]["behavior"],
        name4=npc_list[3]["name"], behavior4=npc_list[3]["behavior"],
        name5=npc_list[4]["name"], behavior5=npc_list[4]["behavior"]
    )

    # LLM 호출 및 응답 수신
    response = llm.invoke(prompt).content.strip()
    print(response)
    try:
        route_data = json.loads(response)
        return route_data  # dict 형태로 반환
    except json.JSONDecodeError:
        print("JSON 파싱 실패! 출력 내용:")
        print(response)
        return {}