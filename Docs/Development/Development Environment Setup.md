### Background Service Setup:

You'll want to install Mailhog and run it, emails from the background service will go here so you can receive those locally.

https://github.com/mailhog/MailHog

Make sure you don't have anything else running on port 1025, for me ProtonMail bridge defaulted to that port so I had to change it.


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