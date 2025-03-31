# LLM 백엔드 서버 환경 가이드 문서
- 최종 업데이트 25.03.31

LLM 백엔드 서버 환경을 처음부터 구축하기 위한 전체 과정을 안내하는 문서입니다.
필자는 Windows, VS Code, Python 3.11.7를 사용해 환경을 구축했습니다.
환경 구축 중에 문제가 있다면, 바로 문의주세요.

---

## 1. VS Code 설치 및 Python 확장 설치
VS Code가 없다면 아래 링크에서 설치한다.
https://code.visualstudio.com/

설치 후, 아래와 같이 Python 확장을 설치한다.

1. VS Code 실행
2. 좌측 Extensions (확장 프로그램) 탭 클릭
3. 검색창에 `Python` 입력
4. **"Python (by Microsoft)"** 확장을 설치

또한 **Python 인터프리터가 시스템에 설치되어 있어야** 한다.  
설치가 되어 있지 않다면 아래 공식 사이트에서 Python 3.10 이상 버전을 설치한다. (필자는 3.11.7)
만약 이미 Python이 3.10 미만 버전으로 설치되어있다면, 제거하고 3.10 이상 버전을 설치하는 것을 권장한다.
(버전이 두 가지여도 가능은 하나, 가상 환경을 구축할 때 버전을 명시해줘야 하므로 번거로움)

https://www.python.org/downloads/

설치 시 반드시 `Add Python to PATH` 옵션을 체크한다.

---

## 2. 프로젝트 가상환경 설정

GitHub 저장소를 클론한 후, VS Code에서 ..\2025-1-LLMJoljakGame\LLMBackEnd 을 Workspace로 지정한다.

가상환경은 이 Workspace 디렉토리에서 `.venv`라는 이름으로 생성할 것이다.
VS Code의 상단 메뉴에서 Terminal > New Terminal (Ctrl + `)을 클릭해 터미널을 실행하고, 아래의 텍스트를 입력한다.

```bash
python -m venv .venv
```

완료되었다면, 가상환경 활성화를 위해 다음 명령어를 입력한다.

```bash
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope Process
.venv\Scripts\Activate.ps1
```

터미널에 `(.venv)` 표시가 나타나면 가상환경이 활성화된 것이다.

---

## 3. 패키지 설치

가상환경이 활성화된 터미널 상태에서 아래 명령어로 필요한 패키지를 설치한다.

```bash
pip install --upgrade pip
pip install -r requirements.txt
```

여기서 오류 메세지가 나오고 제대로 설치가 되지 않은 것 같다면, 다시 확인해보아야 한다.

---

## 4. OpenAI API 키 등록

`2025-1-LLMJoljakGame\LLMBackEnd` 프로젝트 폴더에 `.env` 파일을 생성하고, 다음과 같이 API 키를 입력해야한다.
만들고 메모장 등으로 편집해서 넣어주면 된다.

```
OPENAI_API_KEY=sk-xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
```

'sk~' 부분은 외부에 노출되면 안되는 API 키로, 개별적으로 넣어줄 예정이다.
여기까지 되었다면, 환경은 모두 구축되었다.
---

## 5. 서버 실행

이제 아래 명령어를 터미널에 입력해, FastAPI를 이용한 서버를 구동할 수 있다.

```bash
uvicorn main:app --reload
```

터미널에 아래와 같은 메시지가 출력되면 정상적으로 서버가 실행된 것이다.

```
Uvicorn running on http://127.0.0.1:8000
```

이제 아래 주소를 들어가면, 본인의 로컬에서 실행된 서버를 확인할 수 있다.

```
http://127.0.0.1:8000/docs
```

---

## 6. 백엔드 폴더 구조 예시

```
LLMBackEnd/
├── main.py
├── npc_agent.py
├── graph_flow.py
├── game_master.py
├── prompts/
│   └── npc_template.txt
├── requirements.txt
├── .env                ← 직접 생성
├── .venv/              ← 가상환경 (Git에 포함되지 않음)
├── .gitignore
```
