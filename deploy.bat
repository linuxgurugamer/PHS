
@echo off

set H=R:\KSP_1.3.1_dev
set GAMEDIR=TRP-Hire

echo %H%

copy /Y "%1%2" "GameData\%GAMEDIR%"\Plugins
copy /Y %GAMEDIR%.version GameData\%GAMEDIR%

mkdir "%H%\GameData\%GAMEDIR%"
xcopy /y /s  /i GameData\%GAMEDIR% "%H%\GameData\%GAMEDIR%"
