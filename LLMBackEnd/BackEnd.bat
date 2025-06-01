@echo off

REM 배치 파일 위치로 이동
cd /d "%~dp0"

call .venv\Scripts\activate.bat

python gui_launcher.py

