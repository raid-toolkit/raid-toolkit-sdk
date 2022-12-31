@echo off
setlocal
mkdir %~dp0out 2>nul

:: msiexec /x out\RaidToolkit.msi

:: %~dp0out\RaidToolkit.exe /uninstall /log %~dp0out\install.log

set Prerelease=1
set AppVersion=2.12.9
set BuildConfiguration=Debug
set BuildOutputDirectory=..\Application\Raid.Toolkit\bin\x64\%BuildConfiguration%\net6.0-windows10.0.19041.0\win10-x64

heat.exe dir %BuildOutputDirectory% -cg RaidToolkitBinaries -out out\RaidToolkitBinaries.wxs -dr BINDIR -srd -suid -sreg -g1 -var env.BuildOutputDirectory -ag
:: heat.exe reg classes.reg -out out\RaidToolkitRegistry.wxs -srd -suid -sreg -g1 -ag

candle.exe -nologo -o %~dp0out\ %~dp0out\RaidToolkitBinaries.wxs %~dp0Product.wxs %~dp0Registry.wxs %~dp0Bundle.wxs -ext WixBalExtension
light.exe -nologo -o %~dp0out\RaidToolkit.msi %~dp0out\Product.wixobj %~dp0out\Registry.wixobj %~dp0out\RaidToolkitBinaries.wixobj -ext WixBalExtension
light.exe -nologo -o %~dp0out\RaidToolkit.exe %~dp0out\Bundle.wixobj -ext WixBalExtension

:: %~dp0out\RaidToolkit.exe /install /log %~dp0out\install.log
