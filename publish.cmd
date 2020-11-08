:<<"::SHELLSCRIPT"
@ECHO OFF
GOTO :CMDSCRIPT

::SHELLSCRIPT
#!/bin/bash
os=$1
f="-f netcoreapp3.1"
c="-c Release"
if [[ $os ]]; then
  r="-r $os-x64"
elif [[ $(uname) = "Darwin" ]]; then
  r="-r osx-x64"
elif [[ -f /etc/os-release ]]; then
  . /etc/os-release
  NAME="$(tr '[:upper:]' '[:lower:]' <<< $NAME)"
  r="-r $NAME.$VERSION_ID-x64"
fi
echo SHELLSCRIPT
echo ==== Publishing Client ====
echo dotnet publish ${c} ${r} ${f} --no-restore --self-contained=false -o dist/$os/client src/UOStudio.Client/UOStudio.Client.csproj
dotnet publish ${c} ${r} ${f} --no-restore --self-contained=false -o dist/$os/client src/UOStudio.Client/UOStudio.Client.csproj
echo.
echo ==== Publishing Server ====
echo dotnet publish ${c} ${r} ${f} --no-restore --self-contained=false -o dist/$os/server src/UOStudio.Server/UOStudio.Server.csproj
dotnet publish ${c} ${r} ${f} --no-restore --self-contained=false -o dist/$os/server src/UOStudio.Server/UOStudio.Server.csproj
exit $?

:CMDSCRIPT
SET f=-f netcoreapp3.1
SET c=-c Release

IF "%~1" == "" (
  SET r=-r win-x64
  SET ro=win-x64
) ELSE (
  SET r=-r %~1-x64
  SET ro=%~1-x64
)
echo ==== Publishing Client ====
echo dotnet publish %c% %r% %f% --self-contained=false -o dist/%ro%/client src/UOStudio.Client/UOStudio.Client.csproj
dotnet publish %c% %r% %f% --self-contained=false -o dist/%ro%/client src/UOStudio.Client/UOStudio.Client.csproj
echo.
echo ==== Publishing Server ====
echo dotnet publish %c% %r% %f% --self-contained=false -o dist/%ro%/server src/UOStudio.Server/UOStudio.Server.csproj
dotnet publish %c% %r% %f% --self-contained=false -o dist/%ro%/server src/UOStudio.Server/UOStudio.Server.csproj
