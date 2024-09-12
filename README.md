# Bones [![.NET Build & Test](https://github.com/CorruptComputer/Bones/actions/workflows/build-and-test.yml/badge.svg?branch=develop)](https://github.com/CorruptComputer/Bones/actions/workflows/build-and-test.yml)
This is just something I'm working on in my spare time. 
I will not be accepting any public pull requests to here.

[![Quality gate](https://sonarcloud.io/api/project_badges/quality_gate?project=CorruptComputer_Bones)](https://sonarcloud.io/summary/new_code?id=CorruptComputer_Bones)

## Development Environment Setup
### Background Service Setup:
You'll want to install Mailhog and run it, emails from the background service will go here so you can receive those locally.
https://github.com/mailhog/MailHog

### Api Setup:
```bash
cd Backend\Bones.Api
dotnet tool restore
```

### WebUI Setup:
```bash
cd Frontend\Bones.WebUI
sudo dotnet workload install wasm-tools
cd ..
cd Bones.Api.Client
dotnet tool restore
```

### DB Setup:
```bash
cd Backend\Bones.Database
dotnet tool restore
sudo su postgres
initdb -D /var/lib/postgres/data --auth-local=trust --locale=C.UTF-8 --encoding=UTF8
createdb BonesDb
```

When running it you'll want to run them in this order:
- Bones.BackgroundService
- Bones.Api
- Bones.WebUI