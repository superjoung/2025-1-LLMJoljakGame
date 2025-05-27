from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
from graph_flow import response_graph
from npc_agents.npc_route_planner import plan_full_game_data
from evidence_submit import evidence_graph
from npc_agents.npc_last_statement import generate_final_statements_from_setup
import chromadb
from chromadb.config import Settings
import uuid
import json

# FastAPI 초기화
app = FastAPI()

# ChromaDB 초기화
client = chromadb.Client(Settings(anonymized_telemetry=False))
collection = client.get_or_create_collection(name="memory")

# --- 요청 입력 정의 ---

class UserInput(BaseModel):
    input: str
    npc: str

class EvidenceInput(BaseModel):
    npc: str
    evidence: str  # 예: "E_8"


# --- 대화 API ---
@app.post("/ask")
async def ask_npc(user_input: UserInput):
    try:
        memories = collection.query(
            query_texts=[user_input.input],
            n_results=3,
            where={"npc": user_input.npc},
        )
        retrieved = memories.get("documents", [[]])[0]
        print(f"[memory] 검색된 기억: {retrieved}")

        compiled_graph = response_graph()
        result = compiled_graph.invoke({
            "input": user_input.input,
            "npc": user_input.npc,
            "memory_used": retrieved,
            "allowed": True,
            "response": None
        })

        return {
            "npc": user_input.npc,
            "response": result.get("response", "")
        }

    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))


# --- 증거 제출 API ---
# evidence.json 로딩 (최초 한 번만)
with open("evidence.json", "r", encoding="utf-8") as f:
    evidence_list = json.load(f)
evidence_map = {item["id"]: item["name"] for item in evidence_list}

@app.post("/submit_evidence")
async def submit_evidence(evidence_input: EvidenceInput):
    try:
        # 1. 증거 이름 조회
        clue_id = evidence_input.evidence
        clue_name = evidence_map.get(clue_id)

        if not clue_name:
            raise HTTPException(status_code=404, detail=f"증거 ID '{clue_id}'를 찾을 수 없습니다.")

        # 2. LangGraph 실행
        compiled_graph = evidence_graph()
        result = compiled_graph.invoke({
            "npc": evidence_input.npc,
            "evidence": { "id": clue_id }  # 이름은 내부에서 evidence_graph가 채움
        })

        return {
            "npc": evidence_input.npc,
            "response": result["response"]
        }

    except Exception as e:
        raise HTTPException(status_code=500, detail=f"증거 제출 실패: {str(e)}")


# --- 설정 및 턴 생성 ---
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


@app.post("/generate_turn_data")
async def generate_turn_data():
    try:
        game_data = plan_full_game_data()
        return game_data
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"게임 데이터 생성 실패: {str(e)}")

@app.post("/final_statements")
async def generate_final_statements_api():
    try:
        result = generate_final_statements_from_setup()
        return result
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"최종 발언 생성 실패: {str(e)}")

from chief import generate_chief_statement

@app.get("/chief_statement")
def get_chief_statement():
    try:
        result = generate_chief_statement()
        return {"statement": result}
    except Exception as e:
        return {"error": str(e)}
    