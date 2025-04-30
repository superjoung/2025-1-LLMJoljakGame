from typing import Dict, List

class NPCState:
    def __init__(self):
        self.stress: Dict[str, int] = {}
        self.anxiety: Dict[str, int] = {} 
        self.chat_history: Dict[str, List[str]] = {}

    def init_npcs(self, npc_names: List[str]):
        for name in npc_names:
            self.stress[name] = 0
            self.anxiety[name] = 0 
            self.chat_history[name] = []

    # --- 스트레스 관리 ---
    def add_stress(self, name: str, amount: int = 1):
        self.stress[name] = min(100, self.stress.get(name, 0) + amount)
    def get_stress(self, name: str) -> int:
        return self.stress.get(name, 0)

    # --- 불안감 관리 ---
    def add_anxiety(self, name: str, amount: int = 1):
        self.anxiety[name] = min(100, self.anxiety.get(name, 0) + amount)
    def get_anxiety(self, name: str) -> int:
        return self.anxiety.get(name, 0)

    # --- 대화 기록 관리 ---
    def add_history(self, name: str, message: str):
        if name not in self.chat_history:
            self.chat_history[name] = []
        self.chat_history[name].append(message)

    def get_history(self, name: str) -> List[str]:
        return self.chat_history.get(name, [])

# 싱글톤 인스턴스
npc_state = NPCState()
