#!/usr/bin/env bash

cd Core/Bones.Database
dotnet tool update dotnet-ef
dotnet tool restore

cd ../../Frontend/Bones.Api.Client/
dotnet tool update nswag.consolecore
dotnet tool restore

cd ../Bones.WebUI/
sudo dotnet workload restore

cd ../../Services/Bones.Api/
dotnet tool update swashbuckle.aspnetcore.cli
dotnet tool update dotnet-rpm
dotnet tool update dotnet-deb
dotnet tool restore

cd ../Bones.BackgroundService/
dotnet tool update dotnet-rpm
dotnet tool update dotnet-deb
dotnet tool restore
