@echo off

mkdir %~dp0out 2>nul

:: msiexec /x out\RaidToolkit.msi

set BuildOutputDirectory=..\Application\Raid.Toolkit\bin\x64\Release\net6.0-windows10.0.19041.0\win10-x64

heat.exe dir %BuildOutputDirectory% -cg RaidToolkitBinaries -out RaidToolkitBinaries.wxs -dr INSTALLDIR -srd -suid -sreg -g1 -var env.BuildOutputDirectory -gg

candle.exe -nologo -o %~dp0out\ %~dp0RaidToolkitBinaries.wxs %~dp0Product.wxs
light.exe -nologo -o %~dp0out\RaidToolkit.msi %~dp0out\Product.wixlib %~dp0out\RaidToolkitBinaries.wixobj

:: msiexec /i %~dp0out\RaidToolkit.msi /log %~dp0out\install.log