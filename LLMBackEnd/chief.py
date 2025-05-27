import os
import json
from dotenv import load_dotenv
from langchain_openai import ChatOpenAI

load_dotenv()

llm = ChatOpenAI(
    model="gpt-3.5-turbo",
    temperature=0.7,
    openai_api_key=os.getenv("OPENAI_API_KEY")
)

def generate_chief_statement() -> str:
    # setup.json 로드
    with open("setup.json", "r", encoding="utf-8") as f:
        setup = json.load(f)

    # 프롬프트 템플릿 로드
    with open("prompts/chief_prompt.txt", "r", encoding="utf-8") as f:
        prompt_template = f.read()

    # 플레이스홀더 치환
    prompt = prompt_template.replace("{village}", setup["village"]).replace("{event}", setup["event"])

    # LLM 호출
    response = llm.invoke(prompt)
    content = response.content if hasattr(response, "content") else str(response)

    return content
