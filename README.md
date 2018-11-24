# Gainz

> *&quot;One requires a solid foundation to make big gainz.&quot;*
> \- Some Random Internet User

Gainz is the infrastructure project for Nerdy Mishka projects.

## Goals

Primary Directive: Create tools, scripts, and packages that reduce setup and
maintance friction.

- Environment Setup Scripts
- Policy Docs
- Documentation Templates
- Packages

## Repository Locations

```powershell
[System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Tls12
$uri = "https://raw.githubusercontent.com/nerdymishka/gainz/master/scripts/Install-GitRepo.ps1"
iex (Invoke-WebRequest $uri -UseBasicParsing).Content
```

- Gitlab: https://gitlab.com/nerdymishka/gainz.git
- Github: https://github.com/nerdymishka/gainz.git
- Azure DevOps (vsts): https://nerdymishka.visualstudio.com/gainz/_git/gainz



## License

Copyright 2016-2018 Nerdy Mishka

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.