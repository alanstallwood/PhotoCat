# Agent setup notes

## .NET SDK installation
If the `dotnet` CLI is missing in this environment, install the .NET SDK before building or running tests.

```bash
# Ubuntu/Debian
wget https://packages.microsoft.com/config/ubuntu/$(. /etc/os-release && echo "$VERSION_ID")/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb
sudo apt-get update
sudo apt-get install -y dotnet-sdk-10.0
```

After install, validate:

```bash
dotnet --version
dotnet test PhotoCat.slnx
```
