@echo off
echo Compiling....
dotnet publish -r linux-x64 -c Release --self-contained 
echo Marking Yuki as executable...
wsl chmod +x /mnt/d/Development/Github/Yuki/Yuki/bin/Release/netcoreapp2.1/linux-x64/publish/Yuki
echo Done!