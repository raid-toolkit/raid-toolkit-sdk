@echo off
thirdlicense --project %~dp0..\SDK.sln --output %~dp0..\ThirdPartyNotice.txt
type %~dp0il2cppdumperlicense.txt >> %~dp0..\ThirdPartyNotice.txt