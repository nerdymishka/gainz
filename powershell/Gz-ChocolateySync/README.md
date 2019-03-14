# Gz-ChocolateySync Module

This module will install chocolatey, boxstarter, chocolatey packages and boxstarter packages
based on a JSON configuration file.  

- **chocolatey** - The node for the configuration.
- **update** - Intructs chocolately to update outdated modules in the config.
- **packages** - Packages that should be installed. This takes an array of package commands
  either as a string or a object with settings.  
- **sources** - Adds, Updates, or Removes feeds/sources for chocolatey packages.
- **install** - Configures chocolatey installations.

## Chocolatey Powershell Invoke

### Remote Example

```powershell
Sync-Chocolatey -Uri "https://raw.gitlab.com/repo/project/install.json"
```

### Local Example

```powershell
Sync-Chocolatey -Uri "./chocolatey-sync.json"
```

## Simple Configuration

```json
{
    "chocolatey": {
        "update": true,
        "packages": [
            {
                "name": "git",
                "yes": true,
                "params": {
                    "NOGITLSF": true
                }
            },
            "keepass"
        ]
    }
}
```

## BoxStarter Powershell Invoke

### Remote BoxStarter Config Example

```powershell
# enables the computer or VM to reboot.
$cred = Get-Credential
Sync-Chocolatey -Uri "https://raw.gitlab.com/repo/project/install.json" -Credential $cred
```

### Local BoxStarter Config Example

```powershell
# enables the computer or VM to reboot.
$cred = Get-Credential
Sync-Chocolatey -Uri "./chocolatey-sync.json" -Credential $cred
```

## BoxStarter Configuration

- **boxstarter** - The node that instructs the sync to install Boxstarter if it is not installed.
- **feeds** - Sets the BoxStarter feeds for packages. This can be a string or an array.
- **packages** - Sets the package to install. This can be a string or an array. (Only one package is currently supported)

```json
{
    "boxstarter": {
        "feeds": ["feed1"],
        "packages": ["package1"]
    }
}
```

## Complex Chocolatey Configuration

```json
{
    "chocolatey": {
        "install": {
            "path": "optional",
            "toolsLocation": "optional",
            "downloadUrl":  "optional",
            "version":  "optional",
            "useWindowsCompression":  false ,
            "proxy": { // optional
                "ignore": false,
                "location": "",
                "user": "",
                "password": ""
            }
        },
        "sources": {
            "feed1": "string",
            "feed2": {
                "uri": "string",
                "encrypted": true
            },
            "feed3": {
                "uri": "string",
                "user": true,
                "password": true
            }
            "feed4": { "remove": true}
        },
        "update": true,
        "packages": [
            "git",
            "another.package --params=\"'/INSTALLDIR=path/to/install/dir'\"",
            {
                "name": "yetanotherpackage",
                "params": {
                    "INSTALLDIR": "path/to/install"
                },
                "ignoreChecksum": true
            }
        ]
    }
}
```