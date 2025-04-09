import os
import json
from dotenv import load_dotenv
from langchain_openai import ChatOpenAI

load_dotenv()

llm = ChatOpenAI(
    model="gpt-3.5-turbo",
    temperature=0.9,
    openai_api_key=os.getenv("OPENAI_API_KEY")
)

def load_prompt(file_path: str) -> str:
    with open(file_path, "r", encoding="utf-8") as f:
        return f.read()

def generate_game_setup() -> dict:
    prompt = load_prompt("prompts/social_master_prompt.txt")
    response = llm.invoke(prompt)
    content = response.content if hasattr(response, "content") else str(response)

    setup = json.loads(content)
    with open("setup.json", "w", encoding="utf-8") as f:
        json.dump(setup, f, indent=2, ensure_ascii=False)

    return setup
