@echo off

mkdir %~dp0out 2>nul

msiexec /x out\RaidToolkit.msi

candle.exe -nologo -o %~dp0out\ %~dp0RaidToolkitBinaries.wxs %~dp0Product.wxs
light.exe -nologo -o %~dp0out\RaidToolkit.msi %~dp0out\Product.wixlib %~dp0out\RaidToolkitBinaries.wixobj

msiexec /i %~dp0out\RaidToolkit.msi /log %~dp0out\install.log