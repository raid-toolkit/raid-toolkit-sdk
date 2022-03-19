:: taskkill /F /IM dotnet.exe
dotnet build -v d -nodeReuse:false /p:RTKTasks=true  > %~dp0..\..\..\output.log