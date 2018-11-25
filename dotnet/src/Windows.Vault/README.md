# 🔐  NerdyMishka.Windows.Vault 🔐

Windows Credential Store access done right.

## Overview

This library can access the Windows Credential Store and will keep passwords or certificates
safe in memory until the data is decrypted.

By default the password, certficate, or other key data types are stored encrypted in memory with
Salsa20. A consumer of this library can then call `GetBlobAsSecureString`, `GetBlobAsBytes`,
`GetBlobAsChars` or `GetBlobAsString`. Its recommended to avoid calling `GetBlobAsString` unless
absolutely necessary.

## Repos

- [Gitlab](https://gitlab.com/nerdymishka/gainz/tree/master/dotnet/src/Windows.Vault)
- [Github](https://github.com/nerdymishka/gainz/tree/master/dotnet/src/Windows.Vault)
- [Azure DevOps](https://nerdymishka.visualstudio.com/_git/gainz?path=%2Fdotnet%2Fsrc%2FWindows.Vault&version=GBmaster)

Github is for community interation such as issues and pull requests. Azure DevOps is for
build and release pipelines. Gitlab is the main source repo and will hold another set
of build and release pipelines. 

## Code Samples

```csharp
using NerdyMishka.Vault.Windows;

// elsewhere

var credentials = VaultManager.List();
foreach(var c in credentials) {
    Console.WriteLine(c.Key);
}

// create an entry.
var next = VaultManager.Create();
next.Key = "gainz/test";

// set blob will take a string, char[], byte[] and SecureString.
next.SetBlob("my-great-and-terrible-password");
VaultManager.Write(next);

// find an entry.
var findOne = VaultManager.Read("gainz/test");
if(findOne == null)
    throw new System.Exception("entry for 'gainz/test' could not be found.");

var ss = findOne.GetBlobAsSecureString();
// do something with secure string.

// delete an entry.
VaultManager.Delete(findOne.Key);
```

## Release Notes

- **0.1.0**: Gather early feedback on the API.

## License

Copyright 2016 - 2018 Nerdy Mishka

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.