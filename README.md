# Bones
This is just something I'm working on in my spare time. 
I will not be accepting any public pull requests to here.

---

WebUI Setup:
```bash
sudo dotnet workload install wasm-tools
```


DB Setup:

```bash
sudo su postgres
initdb -D /var/lib/postgres/data --auth-local=trust --locale=C.UTF-8 --encoding=UTF8
createdb BonesDb
```