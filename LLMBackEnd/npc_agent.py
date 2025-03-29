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

npc_prompt_template = PromptTemplate.from_template(
    open("prompts/npc_template.txt", encoding="utf-8").read()
)
def respond_as_npc(npc_name, personality, context, player_input):
    prompt = npc_prompt_template.format(
        npc=npc_name,
        personality=personality,
        context=context,
        input=player_input
    )
    return llm.predict(prompt)
