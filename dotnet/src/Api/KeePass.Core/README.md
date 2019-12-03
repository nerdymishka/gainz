# ![Document Format KeePass](docs/images/document-format-keepass-64.png) DocumentFormat.KeePass

An alternative KeePass DOM and file format library built on .NET Standard

## Goals

Provide a lightweight cross platform .NET API to work 
with the KeePass (2x) xml file format under a less restrictive
opensource license.  Its not the scope of this project to 
provide a GUI or the same functionality as the original 
KeePass Project.     

A side goal is to enable scenarios that are not enabled
by the keepass project.  

## Notes

KeePass uses DPAPI, the windows DataProtection API for tying a KeePass
database to a user account.  

.NET Standard only supports DPAPI for code that executes on Windows.

In order to suppliment other platforms, NerdyMishka.KeePass.DataProtection
provides support windows support and a different xplat provider that generates
a private key and uses that for encryption for the `MasterUserAccountKey` and
for protecting data in memory.


## TODO
 - [ ] Futher enable plugins. 
 - [ ] Add methods to make the library easier to work with
 - [ ] Test custom data
 - [x] Enable export of binaries.
 - [ ] Enable merge functionality.
 - [x] Add a close method for package.

**references:**
 - http://keepass.info/help/base/security.html
 - http://blog.sharedmemory.fr/en/2014/04/30/keepass-file-format-explained/
 - https://www.sysorchestra.com/2015/06/20/reading-a-keepass-file-from-go/
 - https://github.com/cternes/openkeepass (apache license)
 
 
## Project Repos
 - [Gitlab](https://gitlab.com/nerdymishka/DocumentFormat.KeePass.git) (Primary)
 - [Github](https://github.com/nerdymishka/DocumentFormat.KeePass.git)

## License

   Copyright 2016 Nerdy Mishka

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
   
## Logo

Lock graphic by <a href="http://www.freepik.com/">Freepik</a> from
<a href="http://www.flaticon.com/">Flaticon</a> is licensed under
<a href="http://creativecommons.org/licenses/by/3.0/" title="Creative Commons BY 3.0">CC BY 3.0</a>. 

Made with <a href="http://logomakr.com" title="Logo Maker">Logo Maker</a>