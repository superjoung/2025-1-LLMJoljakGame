from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
from graph_flow import game_app
import chromadb
from chromadb.config import Settings
import uuid
import json
import os

# FastAPI 초기화
app = FastAPI()

# ChromaDB 메모리 기반 초기화
client = chromadb.Client(Settings(anonymized_telemetry=False))
collection = client.get_or_create_collection(name="memory")

# 입력 포맷 정의
class UserInput(BaseModel):
    input: str
    npc: str

# 대화 요청 API
@app.post("/ask")
async def ask_npc(user_input: UserInput):
    try:
        # 1. 과거 기억 검색
        memories = collection.query(
            query_texts=[user_input.input],
            n_results=3,
            where={"npc": user_input.npc},
        )
        retrieved = memories.get("documents", [[]])[0]
        print(f"[memory] 검색된 기억: {retrieved}")

        # 2. 동적 그래프 생성 (요청된 NPC에 따라)
        compiled_graph = game_app()

        # 3. 그래프 실행 (입력 + 기억 포함)
        result = compiled_graph.invoke({
            "input": user_input.input,
            "npc": user_input.npc,
            "memory_used": retrieved,
            "allowed": True,      
            "response": None 
        })

        # 응답 저장
        response = result.get("response", "")
        if user_input.npc == "사회자":
            content_to_save = f"{user_input.npc}: {user_input.input}"
        else:
            content_to_save = f"{user_input.npc}: {response}"

        collection.add(
            documents=[content_to_save],
            metadatas=[{"npc": user_input.npc}],
            ids=[str(uuid.uuid4())]
        )

        return {
            "npc": user_input.npc,
            "response": response,
            "memory_used": retrieved
        }

    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))


# 사건 초기 설정 생성용
@app.post("/generate_setup")
async def generate_setup():
    try:
        from setup_generator import generate_game_setup
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
