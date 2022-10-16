@echo off

mkdir %~dp0out 2>nul

:: msiexec /x out\RaidToolkit.msi

%~dp0out\RaidToolkit.exe /uninstall /log %~dp0out\install.log

set Prerelease=1
set AppVersion=2.9.9
set BuildOutputDirectory=..\Application\Raid.Toolkit\bin\x64\Release\net6.0-windows10.0.19041.0\win10-x64

heat.exe dir %BuildOutputDirectory% -cg RaidToolkitBinaries -out RaidToolkitBinaries.wxs -dr INSTALLDIR -srd -suid -sreg -g1 -var env.BuildOutputDirectory -ag

candle.exe -nologo -o %~dp0out\ %~dp0RaidToolkitBinaries.wxs %~dp0Product.wxs %~dp0Bundle.wxs -ext WixBalExtension
light.exe -nologo -o %~dp0out\RaidToolkit.msi %~dp0out\Product.wixobj %~dp0out\RaidToolkitBinaries.wixobj -ext WixBalExtension
light.exe -nologo -o %~dp0out\RaidToolkit.exe %~dp0out\Bundle.wixobj -ext WixBalExtension

:: msiexec /i %~dp0out\RaidToolkit.msi /log %~dp0out\install.log

%~dp0out\RaidToolkit.exe /install /log %~dp0out\install.log