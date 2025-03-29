from fastapi import FastAPI, Request
from pydantic import BaseModel
from graph_flow import game_app

app = FastAPI()
compiled_graph = game_app.compile()

class UserInput(BaseModel):
    input: str
    npc: str

@app.post("/ask")
async def ask_npc(user_input: UserInput):
    result = compiled_graph.invoke({
        "input": user_input.input,
        "npc": user_input.npc,
        "chat_history": []
    })
    return result
