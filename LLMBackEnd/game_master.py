from langchain.chat_models import ChatOpenAI
from langchain.prompts import PromptTemplate
import os
from dotenv import load_dotenv

load_dotenv()
api_key = os.getenv("OPENAI_API_KEY")
llm = ChatOpenAI(
    model="gpt-4",
    temperature=0.7,
    openai_api_key=api_key
)

template = PromptTemplate.from_template(open("prompts/game_setup.txt", encoding="utf-8").read())

def generate_case():
    return llm.predict(template.format())
