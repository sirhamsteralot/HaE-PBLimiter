@echo off

REM Location of your SpaceEngineers.exe
mklink /J Bin64 "C:\Program Files (x86)\Steam\steamapps\common\SpaceEngineers\Bin64"

REM Location of your workshop
mklink /J workshop "C:\Program Files (x86)\Steam\steamapps\workshop"

REM Location of your Torch
mklink /J Torch "C:\Users\SirHamsterAlot\Documents\Space Engineers\Torch\Torch v1.3.1.303"

pause