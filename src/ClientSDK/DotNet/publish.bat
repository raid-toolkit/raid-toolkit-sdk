@echo off
nbgv get-version --variable=NuGetPackageVersion>%temp%\version.txt
set /P Version=<%temp%\version.txt
dotnet nuget push bin\Release\Raid.Client.%Version%.nupkg --source raid-toolkit 