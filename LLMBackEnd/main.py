from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
from graph_flow import game_app
from setup_generator import generate_game_setup
import json

app = FastAPI()
compiled_graph = game_app()

class UserInput(BaseModel):
    input: str
    npc: str

@app.post("/ask")
async def ask_npc(user_input: UserInput):
    try:
        result = compiled_graph.invoke({
            "input": user_input.input,
            "npc": user_input.npc
        })
        return {
            "npc": user_input.npc,
            "response": result.get("response", "응답 없음")
        }
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

@app.post("/generate_setup")
async def generate_setup():
    try:
        setup = generate_game_setup()
        if not setup:
            with open("setup.json", "r", encoding="utf-8") as f:
                setup = json.load(f)

        return {
            "message": "사건 및 용의자 설정 생성 완료",
            "setup": setup
        }
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Setup 생성 실패: {str(e)}")
