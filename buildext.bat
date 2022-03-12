taskkill /F /IM dotnet.exe
cmd /c rimraf **/obj/**
cmd /c rimraf **/bin/**
dotnet clean
dotnet build -v d -nodeReuse:false /p:RTKTasks=true