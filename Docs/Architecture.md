# Project Architecture
## Folder Structure
The `./Core/` directory contains libraries.
The `./Services/` directory contains runnable applications that make up the Bones platform.
The `./Frontend/` directory contains the web frontend for the Bones platform.
The `./Tests/` directory contains unit and integration tests for the Bones platform.

## Project References
./Services/Bones.BackgroundService
./Services/Bones.Api
-> ./Core/Bones.Logic
  -> ./Core/Bones.Database
    -> ./Core/Bones.Shared.Backend
      -> ./Core/Bones.Shared

./Services/Bones.AppHost
-> ./Services/Bones.Api
-> ./Services/Bones.BackgroundService
-> ./Frontend/Bones.WebUI

./Frontend/Bones.WebUI
-> ./Core/Bones.Api.Client
  -> ./Core/Bones.Shared

./Tests/UnitTests/Core/Bones.Logic.UnitTests
./Tests/UnitTests/Core/Bones.Database.UnitTests
-> ./Tests/UnitTests/Core/Bones.Shared.Backend.UnitTests
  -> ./Tests/UnitTests/Bones.Testing.UnitTests.Shared
    -> ./Services/Bones.Api
    -> ./Core/Bones.Logic
    -> ./Core/Bones.Database
    -> ./Core/Bones.Shared.Backend
    -> ./Tests/UnitTests/Bones.Testing.Shared

./Tests/IntegrationTests/Bones.Api.IntegrationTests
-> ./Core/Bones.Api.Client
-> ./Tests/IntegrationTests/Bones.Testing.IntegrationTests.Shared
  -> ./Services/Bones.AppHost
  -> ./Tests/UnitTests/Bones.Testing.Shared