cd Frontend/Bones.Api.Client/
dotnet tool update nswag.consolecore

cd ../../Services/Bones.Api/
dotnet tool update swashbuckle.aspnetcore.cli
dotnet tool update dotnet-rpm
dotnet tool update dotnet-deb

cd ../Bones.BackgroundService/
dotnet tool update dotnet-rpm
dotnet tool update dotnet-deb