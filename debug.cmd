@echo off

SET msbuild=C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe

if exist src\bin\Debug (rmdir /s /q src\bin\Debug)
%msbuild% -m -t:Build -property:Configuration=Debug src\IronRockUtils.sln
if errorlevel 1 goto :failed

if exist dist (rmdir /s /q dist)
if not exist dist (mkdir dist)

copy /y src\bin\Debug\*.* dist

del dist\*.xml

echo *******************
echo Success!
echo *******************
goto :eof

:failed
echo *******************
echo ERROR: Unable to build project.
echo *******************
goto :eof
