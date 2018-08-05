$vsWebGuids = @{
    "ASP.NET 5" = 	"{8BB2217D-0F2D-49D1-97BC-3654ED321F3B}";
    "ASP.NET MVC 1" = "{603C0E0B-DB56-11DC-BE95-000D561079B0}";
    "ASP.NET MVC 2"	= "{F85E285D-A4E0-4152-9332-AB1D724D3325}";
    "ASP.NET MVC 3"	 = "{E53F8FEA-EAE0-44A6-8774-FFD645390401}";
    "ASP.NET MVC 4" = "{E3E379DF-F4C6-4180-9B81-6769533ABE47}";
    "ASP.NET MVC 5" = "{349C5851-65DF-11DA-9384-00065B846F21}";
    "WebSite" = "{E24C65DC-7377-472B-9ABA-BC803B73C61A}";
}

$vsWpfGuid = "{60dc8134-eba5-43b8-bcc9-bb4bc16c2548}";

$languageGuids = @{
    "C#" = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"
    "C++" = "{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}"
    "F#" = "{F2A71F9B-5D33-465A-A702-920D77279786}"
    "J#" = "{E6FDF86B-F3D1-11D4-8576-0002A516ECE8}"
    "VB.NET" = "{F184B08F-C81C-45F6-A57F-5ABD9991F28F}"
}


$fmgVsProjectFiles = @{
    '.csproj' = '.cs'
    '.vcxproj' = '.cpp'
    '.vbproj' = '.vb'
    '.fsproj' = '.fs'
    '.vsjproj' = '.jsl'
    '.wixproj' = '.wxs'
    '.njsproj' = '.js'
    '.ccproj' = '.csdef'
} 

$testRefs = @("Microsoft.VisualStudio.QualityTools.UnitTestFramework", "xunit", "NUnit.Framework", "NUnitLite")
$testPackageRef = @("Microsoft.NET.Test.Sdk") 

$tsImport = "`$(MSBuildExtensionsPath32)\Microsoft\VisualStudio\v`$(VisualStudioVersion)\TypeScript\Microsoft.TypeScript.targets"
$vslibSdkValue = "Microsoft.NET.Sdk"
$vsWebSdkValue = "Microsoft.NET.Sdk.Web" 
$ns = @{"msb" = "http://schemas.microsoft.com/developer/msbuild/2003"}
$serviceRef = "System.ServiceProcess"

function Read-VisualStudioSolution() {
<#
.SYNOPSIS
    Reads a Visual Studio solution file and  returns meta
    information including the version of Visual Studio 
    for the solution file and a hashtable of projects
    with meta info about the projects.

.DESCRIPTION
    Reads a Visual Studio solution file and  returns meta
    information including the version of Visual Studio 
    for the solution file and a hashtable of projects
    with meta info about the projects.

    The hashtable key will be the project names. The value
    will have meta info about the project including:
    - Name: name of the project
    - File: the relative path to the project file
    - Id: the id of project e.g. guid
    - IsWebProject: true if the project is a web project
    - WebProjectType: the type of web project
    - IsTestProject: true if the project is a test project
    - IsWindowsService: true if the project is a windows service
    - IsCloudService: true if the project is a cloud service
    - IsWixSharpProject: true if the project is a wix sharp project
    - IsWixProject: true if the project is a wix project.
    - Ext: project extension e.g. csproj
    - LanguageExt: the code file extension e.g. .cs
    - IsSdk: true if the project is a dotnet standard/core project
 

.PARAMETER WebProjects
    Optional. The switch instructs the cmdlet to test for
    web projects in the solution.

.PARAMETER WindowsServiceProjects
    Optional. The switch instructs the cmdlet to test for
    windows service projects in the solution.

.PARAMETER TestProjects
    Optional. The switch instructs the cmdlet to test
    for test projects in the solution.

.PARAMETER WixSharpProjects
    Options. The switch instructs the cmdlet to test for
    Wix Sharp projects in the solution.

.EXAMPLE
    PS C:\> info = Read-VisualStudioSolution "$Home/Projects/Project.sln"

.OUTPUTS
    a custom object with meta information about the projects in the
    solution and the version of Visual Studio that created the sln.
#>
    [CmdletBinding()]
    Param(
        [Parameter(Position = 0)]
        [string] $Path,
        [Switch] $WebProjects,
        [Switch] $WindowServiceProjects,
        [Switch] $TestProjects,
        [Switch] $WixSharpProjects,
        [Switch] $All 
    )
    $projects = @{};
    $sln = Get-Item $Path -Ea SilentlyContinue
    
    if(!$sln) {
        throw [System.IO.FileNotFoundException] $Path 
    }

    Write-Host $sln.FullName
    $parentDir = Split-Path $Path 
    $lines = Get-Content ($sln.FullName)

    $version = $null;

    foreach($line in $lines) {

        if($line.StartsWith("VisualStudioVersion =")) {
            $version = $line.Substring($line.IndexOf("=") + 1).Trim()
        }

        if($line.StartsWith("Project(")) {
             $data = $line.Substring($line.IndexOf("=") + 1);
             $data = $data.Split(",")
             if($data.Length -lt 3) {
                 continue;
             }
          
             $projName = $data[0].Trim().Trim('"')

           

             $projFile = $data[1].Trim().Trim('"')
             $projFile = "$parentDir\$projFile"
             $projId = $data[2].Trim().Trim('"')
             $webProject = $false;
             $webProjectType = $false;
             $testProject = $false;
             $windowsServiceProject = $false;
             $wixSharpProject = $false;
             $ext = [System.IO.Path]::GetExtension($projFile)
             $languageExt = $null
             $isSdk = $false;

             if($fmgVsProjectFiles.ContainsKey($ext)) {
                $languageExt = $fmgVsProjectFiles[$ext]
             }

             if(![IO.Path]::HasExtension($projFile)) {
                 continue;
             }

             try {
                    $xml = [xml](Get-Content $projFile)
                    if($xml.DocumentElement.HasAttribute("Sdk")) {
                        $isSdk = $true;
                    }
                } catch {

                }
               

            $xml = $null
            if($WebProjects.ToBool() -or $All.ToBool()) {
     
                $webProject = $false;
                
                if($xml -eq $null) {
                    $xml = [xml](Get-Content $projFile)
                }

                if($xml.DocumentElement.HasAttribute("Sdk")) {
                    $isSdk = $true;
                    $sdk = $xml.DocumentElement.Attributes["Sdk"];
                    
                    if($sdk -eq $vsWebSdkValue) {
                        $webProject = $true;
                        $webProjectType = "ASP.NET Core"
                        $selection = $xml | Select-Xml "//TargetFramework"
                   
                        if($selection) {
                            $value = $selection.Node.Value;
                            $value = $value.Replace("netcoreapp", "");
                            $webProjectType = "$webProjectType $value" 
                        }
                    }
                } else {
                    $selection = $xml | Select-Xml "//msb:ProjectTypeGuids" -Namespace $ns
                    $guids = @();
                    if($selection) {
                        $guids = $selection.Node.InnerText
                    
                        if($guids -and $guids.Length -gt 0) {
                            $guids = $guids.Split(";");
                        }
                    }

                    foreach($guid in $guids) {
                        
                        foreach($key in $vsWebGuids.Keys) {
                            $value = $vsWebGuids[$key];
                        
                            if($value -eq $guid) {
                                $webProject = $true;
                                $webProjectType = $key;
                                break;
                            }
                        }

                        if($webProject) {
                            break;
                        }
                    }
                }
             }

             if($WixSharpProjects.ToBool()-or $All.ToBool()) {
            
                $wixSharpProject = $false;

                if($xml -eq $null) {
                    $xml = [xml](Get-Content $projFile)
                }
              
                $xml | Select-Xml -XPath "//msb:Reference" -Namespace $ns | ForEach-Object {
                    if($wixSharpProject) {
                        return;
                    } 
                    $value = ""
                    if($_.Node.HasAttribute("Include")) {
                        $value = $_.Node.Attributes["Include"].Value;
                    }
                   
                    if($value -eq "WixSharp.Msi") {
                        $selection = $xml | Select-Xml "//msb:OutputType" -Namespace $ns 
                        
                        if($selection) {
                            $value = $selection.Node.InnerText
                            if($value -eq "Exe" -or $value -eq "WinExe") {
                                 $wixSharpProject = $true;
                            }
                        }
                       
                        return;
                    }
                }
            
             }
     
             if($WindowServiceProjects.ToBool() -or $All.ToBool()) {
                $windowsServiceProject = $false;

                if($xml -eq $null) {
                    $xml = [xml](Get-Content $projFile)
                }
                
                $xml | Select-Xml -XPath "//msb:Reference" -Namespace $ns | ForEach-Object {
                    if($windowsServiceProject) {
                        return;
                    }
                    
                    $value = $null;
                    if($_.Node.HasAttribute("Include")) {
                         $value = $_.Node.Attributes["Include"].Value
                    }
                   
                   
                    
                    if($value -eq "System.ServiceProcess") {
                        $selection = $xml | Select-Xml -XPath "//msb:OutputType" -Namespace $ns 
                     

                        if($selection -ne $null) {
                            $value = $selection.Node.InnerText
                           
                            if($value -eq "Exe" -or $value -eq "WinExe") {
                                 $windowsServiceProject = $true;
                                 return;
                            }
                        }
                       
                        return;
                    }
                }
             }

             if($TestProjects.ToBool() -or $All.ToBool()) {
                $testProject = $false;

                if($xml -eq $null) {
                    $xml = [xml](Get-Content $projFile)
                   
                }

                if($xml.DocumentElement.HasAttribute("Sdk")) {
                     $isSdk = $true;
                    $xml | Select-Xml "//PackageReference" | ForEach-Object {
                        if($testProject) {
                            return;
                        }
                        $package = $null
                        if($_.Node.HasAttribute("Include")) {
                             $package = $_.Node.Attributes["Include"].Value;
                        }
                       
                        if($package -eq $testPackageRef) {
                            $testProject = $true;
                        }
                    }
                } else {
                    $xml | Select-Xml "//msb:Reference" -Namespace $ns | ForEach-Object {
                        if($testProject) {
                            return;
                        }

                        $value = ""
                        if($_.Node.HasAttribute("Include")) {
                            $value = $_.Node.Attributes["Include"].Value
                        }
                        
                       
                        foreach($testRef in $testRefs) {
                            if($value.StartsWith($testRef)) {
                                $testProject = $true;
                                break;
                            }
                        }
                    }
                }
             }

             if($xml -and $xml.DocumentElement.HasAttribute("Sdk")) {
                $isSdk = $true;
             }
       

             $model = New-Object PsObject -Property @{
                 'Name' = $projName
                 'File' = $projFile
                 'Id' = $projId
                 'IsWebProject' = $webProject
                 'WebProjectType' = $webProjectType
                 'IsTestProject' = $testProject
                 'IsWindowsService' = $windowsServiceProject
                 'IsCloudService' = $ext -eq '.ccproj'
                 'IsWixSharpProject' = $wixSharpProject
                 'IsWixProject' = $ext -eq ".wixproj"
                 'Ext' = $ext
                 'LanguageExt' = $languageExt
                 'IsSdk' = $isSdk
             }

           $projectDir = Split-Path $projFile
            
            $packageJson = $null;
            $bowerrc = $null;
            $gulpfile = $null;
            $gruntfile = $null;
            if(Test-Path "$projectDir/package.json") {
                    $packageJson = "$projectDir/package.json"
            }
            if(Test-Path "$projectDir/.bowerrc") {
                $bowerrc = "$projectDir/.bowerrc"
            }
            if(Test-Path "$projectDir/gulpfile.js") {
                $gulpfile = "$projectDir/gulpfile.js"
            }
            if(Test-Path "$projectDir/Gruntfile.js") {
                $gruntfile = "$projectDir/Gruntfile.js"
            }
            
            $model | Add-Member -MemberType NoteProperty -Name "PackageJson" -Value $packageJson
            $model | Add-Member -MemberType NoteProperty -Name "Bower" -Value $bowerrc
            $model | Add-Member -MemberType NoteProperty -Name "Gulp" -Value $gulpfile
            $model | Add-Member -MemberType NoteProperty -Name "Grunt" -Value $gruntfile
      
            $nuspec =  Get-Item "$projectDir/*.nuspec" -ErrorAction SilentlyContinue
            if($nuspec) {
                $nuspec = $nuspec.FullName   
            } else {
                $nuspec = $false
            }

          
        
            $model | Add-Member -MemberType NoteProperty -Name "NuSpec" -Value $nuspec
           
        

            $projects.Add($projName, $model);
        }
    }

    $meta = [PSCustomObject] (New-Object PSCustomObject -Property @{
        Version = $version
        Projects = $projects
    })

    return $meta;
}
Set-Alias -Name "Read-VsSolution" -Value Read-VisualStudioSolution
