import os
import json
import random
from dotenv import load_dotenv
from langchain_openai import ChatOpenAI
from collections import defaultdict

load_dotenv()

llm = ChatOpenAI(
    model="gpt-4",
    temperature=0.9,
    openai_api_key=os.getenv("OPENAI_API_KEY")
)

def load_prompt(file_path: str) -> str:
    with open(file_path, "r", encoding="utf-8") as f:
        return f.read()

# 중복된 gender+age_group 조합을 2명 이하로 제한하는 함수
def enforce_npc_diversity(suspects):
    seen = defaultdict(list)

    # 조합별로 분류
    for npc in suspects:
        key = (npc["gender"], npc["age_group"])
        seen[key].append(npc)

    trimmed = []
    for group in seen.values():
        if len(group) > 2:
            trimmed.extend(random.sample(group, 2))  # 최대 2명만 허용용
        else:
            trimmed.extend(group)

    # NPC 수가 부족하면 기존에서 랜덤 복제
    while len(trimmed) < 5:
        trimmed.append(random.choice(trimmed))

    return trimmed[:5]

def generate_game_setup() -> dict:
    prompt = load_prompt("prompts/social_master_prompt.txt")
    response = llm.invoke(prompt)
    content = response.content if hasattr(response, "content") else str(response)

    setup = json.loads(content)

    # suspect 다양성 제한 적용
    setup["suspects"] = enforce_npc_diversity(setup["suspects"])

    with open("setup.json", "w", encoding="utf-8") as f:
        json.dump(setup, f, indent=2, ensure_ascii=False)

    return setup
