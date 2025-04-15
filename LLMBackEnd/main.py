from fastapi import FastAPI
from pydantic import BaseModel
import chromadb
import uuid
from graph_flow import create_dynamic_graph

app = FastAPI()

# 🔧 메모리 기반 DB 초기화 (서버 끄면 삭제됨)
client = chromadb.Client()  # persist_directory 없이 사용
npc_collection = client.get_or_create_collection("npc_memory")
host_collection = client.get_or_create_collection("host_memory")


# 요청 모델
class UserInput(BaseModel):
    input: str
    npc: str

@app.post("/ask")
async def ask_npc(user_input: UserInput):
    compiled_graph = create_dynamic_graph(user_input.npc)
    # 1. 컬렉션 선택
    collection = host_collection if user_input.npc == "사회자" else npc_collection

    # 2. 기억 검색
    results = collection.query(
        query_texts=[user_input.input],
        n_results=1
    )
    memory_used = results["documents"][0] if results["documents"] else ""

    # 3. 사회자면 GPT 호출 생략
    if user_input.npc == "사회자":
        response = f"(기록됨) {user_input.input}"  # 표시용 메시지 (없어도 됨)
    else:
        # 동적 LangGraph 생성 및 실행
        dynamic_graph = create_dynamic_graph(user_input.npc)
        response = dynamic_graph.invoke({
            "input": user_input.input,
            "npc": user_input.npc,
            "chat_history": []
        })

    # 4. 기억 저장
    collection.add(
        documents=[f"{user_input.npc}: {user_input.input if user_input.npc == '사회자' else response}"],
        metadatas=[{"npc": user_input.npc}],
        ids=[str(uuid.uuid4())]
    )

    return {
        "gpt_response": response,
        "memory_used": memory_used
    }

