@echo off
setlocal
SET CurrentDir=%~dp0
set version=%1
if [%1] == [] set version=3.0.5
nuget pack "%CurrentDir%EPiCode.Newsletter.nuspec" -Version %version%

