taskkill /F /IM dotnet.exe
cmd /c rimraf **/obj/**
cmd /c rimraf **/bin/**
dotnet clean
dotnet build -m:1 -v d -nodeReuse:false