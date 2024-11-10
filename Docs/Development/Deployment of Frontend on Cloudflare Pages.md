
Point it at the repo and set these:

Build command: 
```
curl -sSL https://dot.net/v1/dotnet-install.sh > dotnet-install.sh; chmod +x dotnet-install.sh; ./dotnet-install.sh -c 8.0 -InstallDir ./dotnet8; ./dotnet8/dotnet --version; mkdir publish; ./dotnet8/dotnet workload install wasm-tools; ./dotnet8/dotnet publish Frontend/Bones.WebUI/Bones.WebUI.csproj --configuration Release --property PublishDir=/opt/buildhome/repo/publish;
```

Output folder: 
```
publish/wwwroot
```