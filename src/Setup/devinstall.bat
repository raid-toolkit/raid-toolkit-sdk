@echo off
setlocal

%~dp0out\RaidToolkit.exe /uninstall /log %~dp0out\uninstall.log

dotnet build %~dp0..\..\SDK.sln --nologo -m:1 -nodeReuse:false -p:GenerateFullPaths=true -p:Configuration=Debug -p:Platform=x64 -p:PackageOutputPath="%~dp0..\..\publish\nuget"
call build.bat

%~dp0out\RaidToolkit.exe /update /log %~dp0out\install.log
