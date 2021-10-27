# GTFS-Explorer
View an interactive map of all transit routes contained in a General Transit Specification Feed (GTFS) archive.

# Building from Source
## Linux
### 1. Install the .NET Framework

[Find the .NET 3.1 package for your distribution.](https://docs.microsoft.com/en-us/dotnet/core/install/linux)
If your distribution supports Snap packages, use the following command:

`sudo snap install dotnet-sdk --classic --channel=3.1`

Register the `dotnet` command (or an alias of your choice):

`sudo snap alias dotnet-sdk.dotnet dotnet`

Lastly, add the environment variable to your PATH:

`export DOTNET_ROOT=/snap/dotnet-sdk/current`
