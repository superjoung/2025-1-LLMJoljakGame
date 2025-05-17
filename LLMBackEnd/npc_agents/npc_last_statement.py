import json
import os
from langchain_openai import ChatOpenAI
from langchain_core.prompts import PromptTemplate
import chromadb
from chromadb.config import Settings

# LLM 설정
llm = ChatOpenAI(
    model="gpt-3.5-turbo",
    temperature=0.7,
    openai_api_key=os.getenv("OPENAI_API_KEY")
)

# 벡터 DB 설정
client = chromadb.Client(Settings(anonymized_telemetry=False))
collection = client.get_or_create_collection(name="memory")

def get_memory_for_npc(npc_name: str, max_items: int = 3) -> str:
    try:
        results = collection.query(
            query_texts=[npc_name],
            n_results=max_items,
            where={"npc": {"$eq": npc_name}},
        )
        return "\n".join(results.get("documents", [[]])[0])
    except Exception as e:
        print(f"[memory_fetch_error] {npc_name}: {e}")
        return ""

def generate_final_statements_from_setup(setup_path: str = "setup.json") -> dict:
    # setup.json 로드
    with open(setup_path, "r", encoding="utf-8") as f:
        suspects = json.load(f)["suspects"]
    target_npcs = suspects[:5]

    # prompt 변수 만들기
    prompt_vars = {}
    for i, npc in enumerate(target_npcs, start=1):
        prompt_vars[f"name{i}"] = npc["name"]
        prompt_vars[f"behavior{i}"] = npc["personality"]["behavior"]
        prompt_vars[f"emotion{i}"] = npc["personality"]["emotion"]
        prompt_vars[f"occupation{i}"] = npc["occupation"]
        prompt_vars[f"statement{i}"] = npc["statement"]
        prompt_vars[f"is_witch{i}"] = npc["is_witch"]
        prompt_vars[f"memory{i}"] = get_memory_for_npc(npc["name"])

    # 프롬프트 로드 및 포맷
    with open("prompts/final_statement_prompt.txt", "r", encoding="utf-8") as f:
        prompt_template = PromptTemplate.from_template(f.read())

    prompt = prompt_template.format(**prompt_vars)

    # LLM 호출
    response = llm.invoke(prompt).content.strip()

    try:
        return json.loads(response)
    except json.JSONDecodeError as e:
        raise ValueError(f"JSON parsing failed: {e}\nRaw response:\n{response}")
