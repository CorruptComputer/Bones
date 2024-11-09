Create 2 DNS records for your API service:
- api.*
  - A | (ipv4)
  - AAAA | (ipv6)

Create the Fedora VM and SSH into it, and do the following:
```bash
vim .ssh/authorized_keys
# Add your public key in here

vim /etc/ssh/sshd_config
# In here find     PasswordAuthentication yes
# and change it to PasswordAuthentication no

# Next time you restart this system it will require you to use the private key

dnf update
# Go ahead and make sure its up to date

dnf install git dnf5-plugins dotnet-runtime-8.0 aspnetcore-runtime-8.0 postgresql-server postgresql-contrib tmux certbot ufw cronie
# Install some prerequisites

dnf config-manager addrepo --from-repofile=https://cli.github.com/packages/rpm/gh-cli.repo
# Add the GitHub CLI repo to dnf

dnf install gh --repo gh-cli
# Install the GH CLI

vim /etc/hostname
# Set your hostname to api.*, same as the DNS record

ufw enable
ufw allow ssh
ufw allow http
ufw allow https
systemctl enable --now ufw
# Setup the systems firewall to allow these connections, by default fedora firewall blocks everything

systemctl enable postgresql
su postgres
initdb -D /var/lib/pgsql/data --auth-local=trust --locale=C.UTF-8 --encoding=UTF8
exit
systemctl start postgresql
# Enable, init, and start postgres (probably not great to auth-local=trust, but if you want to configure a user figure that out yourself or wait until I deploy this on a real system and change the docs here)

reboot
# Finally go ahead and reboot, grab a cup of coffee and pray you setup the SSH key correctly. Lest you need to destroy this VM and start over.
```

Once its back up and you're able to SSH in:
```bash
gh auth login
# Go ahead and login with any account that has read access to the repo

gh repo clone CorruptComputer/Bones
# Go ahead and clone the repo (there has to be a better way to do this)

cd Bones/
gh release download nightly --dir ../
# Grab the artifacts, change nightly to whatever release you want

dnf install ./Bones.Api.0.0.1-nightly.linux-x64.rpm ./Bones.BackgroundService.0.0.1-nightly.linux-x64.rpm
# Install them

Bones.BackgroundService
# Go ahead and run the background service to make sure it crashes, should say something about "Missing 'BackgroundServiceConfiguration' configuration section."
# This is what we are going to setup now

mkdir ~/bones/
mkdir ~/bones/backgroundService/
cp /usr/share/Bones.BackgroundService/appsettings.Production.sample.json ~/bones/backgroundService/appsettings.Production.json
vim appsettings.Production.json
# Go ahead and setup the parameters in here. Just make sure you check in the sample.json file to see if there are any new configs that need to be set. Or just wait for the app to break and tell you then come back and do it here 3 weeks later (more likely :^)

certbot certonly --standalone --agree-tos --email <put your email here> -d <your api.* domain>
# Now, we need to setup the SSL cert for this

# Also add the following to your crontab to make sure this doesn't expire:
# 0 0 * * * certbot renew

mkdir ~/bones/api/
cp /usr/share/Bones.Api/appsettings.Production.sample.json ~/bones/api/appsettings.Production.json
vim appsettings.Production.json
# Same deal

cd

tmux new -d -s BackgroundService 'cd ~/bones/backgroundService; Bones.BackgroundService;'
tmux new -d -s Api 'cd ~/bones/api; Bones.Api'
# Go ahead and start them, tmux will continue running them until they probably crash for some reason
```
