@echo off

SET msbuild=C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe

if exist src\bin\Release (rmdir /s /q src\bin\Release)
%msbuild% -m -t:Build -property:Configuration=Release src\IronRockUtils.sln
if errorlevel 1 goto :failed

if exist dist (rmdir /s /q dist)
mkdir dist

copy src\bin\Release\*.* dist

del dist\*.xml
del dist\*.pdb

echo *******************
echo Success!
echo *******************
goto :eof

:failed
echo *******************
echo ERROR: Unable to build project.
echo *******************
goto :eof
