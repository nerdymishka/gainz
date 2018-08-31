

### Simple

```json
{
    
    "update": true,
    "packages": {
        "git": true,
        "another.package": "--params=\"'/INSTALLDIR=path/to/install/dir'\"",
        "yet.another": {
            "params": {
                "INSTALLDIR": "path/to/install"
            },
            "ignoreChecksum": true 
        }
    }
}
```

###  Complex
```json
{
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
    "packages": {
        "git": true,
        "another.package": "--params=\"'/INSTALLDIR=path/to/install/dir'\"",
        "yet.another": {
            "params": {
                "INSTALLDIR": "path/to/install"
            },
            "ignoreChecksum": true 
        }
    }
}
```