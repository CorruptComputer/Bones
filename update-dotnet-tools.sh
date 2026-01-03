#!/usr/bin/env bash

dotnet tool update --global --all
dotnet workload update

cd Core/Bones.Database
dotnet tool update dotnet-ef
dotnet tool restore

cd ../../Frontend/Bones.WebUI/
dotnet workload restore

cd ../../Services/Bones.Api/
dotnet tool update swashbuckle.aspnetcore.cli
dotnet tool restore
