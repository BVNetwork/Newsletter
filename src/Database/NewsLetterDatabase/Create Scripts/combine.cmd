@echo off
del combinedcreatescript.sql
copy /b *.sql combinedcreatescript.temp > nul
ren combinedcreatescript.temp combinedcreatescript.sql
echo A script for creating all the tables and procedures has been created for you
pause