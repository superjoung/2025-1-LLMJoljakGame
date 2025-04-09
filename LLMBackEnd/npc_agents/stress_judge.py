from langchain_openai import ChatOpenAI
from dotenv import load_dotenv
import os

load_dotenv()

llm = ChatOpenAI(
    model="gpt-3.5-turbo",
    temperature=0.0,
    openai_api_key=os.getenv("OPENAI_API_KEY")
)

def is_sensitive_question(question: str) -> bool:
    prompt = f"""
다음 플레이어의 질문이 민감하거나 불쾌감을 줄 수 있는 질문인지 판별해줘.
민감한 질문이라면 "true", 아니면 "false"를 반환해.

질문: "{question}"
결과:
"""
    result = llm.invoke(prompt).content.strip().lower()
    return "true" in result
