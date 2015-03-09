@echo off
setlocal
SET CurrentDir=%~dp0
set version=%1
if [%1] == [] set version=3.0.4
nuget pack "%CurrentDir%EPiCode.Newsletter.Examples.nuspec" -Version %version%

