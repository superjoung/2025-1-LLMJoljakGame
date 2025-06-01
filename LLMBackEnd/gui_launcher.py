# gui_launcher.py
import os
import subprocess
import threading
import time
import PySimpleGUI as sg

# -------------- 설정 --------------
# 기본값을 여기에 적어두세요. 필요하다면 .env를 쓰지 않아도 됩니다.
DEFAULT_HOST = "127.0.0.1"
DEFAULT_PORT = "8000"

# uvicorn을 통해 FastAPI 앱을 실행할 때 사용할 모듈 경로
# 예) 프로젝트 루트에 app.py가 있다면 "app:app"
UVICORN_MODULE = "main:app"

# -------------- 전역 변수 --------------
server_process = None  # subprocess.Popen 인스턴스가 들어갈 예정

# -------------- GUI 레이아웃 정의 --------------
layout = [
    [sg.Text("OpenAI API Key:"), sg.InputText(key="-APIKEY-", size=(40, 1))],
    [sg.Text("호스트:"), sg.Input(DEFAULT_HOST, key="-HOST-", size=(15,1)), 
     sg.Text("포트:"), sg.Input(DEFAULT_PORT, key="-PORT-", size=(6,1))],
    [sg.Button("서버 시작", key="-START-"), sg.Button("서버 중지", key="-STOP-", disabled=True)],
    [sg.Multiline("", key="-LOG-", size=(60, 15), disabled=True, autoscroll=True)],
]

window = sg.Window("FastAPI GUI Launcher", layout, finalize=True)

# -------------- 헬퍼 함수 정의 --------------
def append_log(msg: str):
    """GUI의 로그 창에 메시지를 추가함."""
    window["-LOG-"].update(f"{msg}\n", append=True)

def start_server(api_key: str, host: str, port: str):
    """
    uvicorn을 subprocess로 실행한다.
    실행 중인 프로세스는 전역 server_process에 저장됨.
    """
    global server_process

    # 이미 실행 중이면 그냥 리턴
    if server_process is not None:
        append_log("이미 서버가 실행 중입니다.")
        return

    # 환경 변수 설정 (기존 config.py가 OPENAI_API_KEY를 이 환경 변수에서 읽어들인다고 가정)
    os.environ["OPENAI_API_KEY"] = api_key
    append_log(f"환경 변수 OPENAI_API_KEY 설정됨.")

    # uvicorn 커맨드 빌드
    cmd = [
        "uvicorn",
        UVICORN_MODULE,
        "--host", host,
        "--port", port,
        # "--reload",  # 필요하다면 개발 모드로 reload를 켤 수 있음
    ]

    append_log(f"서버를 실행합니다: {' '.join(cmd)}")
    # stdout, stderr를 PIPE로 연결해서 GUI에서 로그를 표시하도록 해도 되고,
    # 일단은 터미널/콘솔에 그대로 출력되도록 두겠습니다.
    server_process = subprocess.Popen(
        cmd,
        stdout=subprocess.PIPE,
        stderr=subprocess.STDOUT,
        text=True,
    )

    # 로그를 비동기로 읽어 들여 GUI 창에 보여 주는 스레드
    def read_stdout():
        if server_process.stdout:
            for line in server_process.stdout:
                append_log(line.rstrip())
        append_log("서버 프로세스가 종료되었습니다.")
        window["-START-"].update(disabled=False)
        window["-STOP-"].update(disabled=True)

    threading.Thread(target=read_stdout, daemon=True).start()

def stop_server():
    """서버 프로세스를 terminate."""
    global server_process
    if server_process is None:
        append_log("실행 중인 서버가 없습니다.")
        return

    append_log("서버를 중지합니다...")
    server_process.terminate()
    # 혹시 좀 더 강제 종료가 필요하다면 다음과 같이 할 수 있음:
    # server_process.kill()
    server_process = None

# -------------- GUI 이벤트 루프 --------------
while True:
    event, values = window.read(timeout=100)

    if event == sg.WIN_CLOSED:
        # 창을 닫을 때 서버가 살아 있으면 종료
        if server_process is not None:
            stop_server()
            time.sleep(0.5)
        break

    # "서버 시작" 버튼 클릭
    if event == "-START-":
        api_key = values["-APIKEY-"].strip()
        host = values["-HOST-"].strip()
        port = values["-PORT-"].strip()

        if not api_key:
            append_log("API Key를 입력해주세요.")
            continue

        try:
            int(port)
        except ValueError:
            append_log("포트 번호는 숫자로만 입력해주세요.")
            continue

        window["-START-"].update(disabled=True)
        window["-STOP-"].update(disabled=False)
        start_server(api_key, host, port)

    # "서버 중지" 버튼 클릭
    if event == "-STOP-":
        stop_server()

# 윈도우 닫기
window.close()
