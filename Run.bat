@echo off
REM Turn off the messy terminal text

echo [1/3] Waking up the AI Engine...
REM 'start /B' runs this silently in the background so no black box stays open
start /B ollama run llama3.2:1b

echo [2/3] Starting the Web Server...
REM Notice the quotation marks around the path! That fixes the "space" issue.
start /B python "Python Backend\game_server.py"

echo [3/3] Launching the Game...
REM The '/WAIT' flag pauses this script right here until the game is closed.
REM (Change "My_Virtual_Photography_Game.exe" to your actual Unity game file)
REM start /WAIT "" "My_Virtual_Photography_Game.exe"

pause

echo Game closed! Cleaning up background processes...
REM The game is closed, so we hunt down the Python server and terminate it.
taskkill /F /IM python.exe >nul 2>&1
taskkill /F /IM ollama.exe >nul 2>&1

exit