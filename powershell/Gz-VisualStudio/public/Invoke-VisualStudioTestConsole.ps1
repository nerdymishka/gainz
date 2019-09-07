function Invoke-VisualStudioTestConsole() {
<#
.SYNOPSIS
    Short description
.DESCRIPTION
    Long description
.EXAMPLE
    PS C:\> <example usage>
    Explanation of what the example does
.INPUTS
    Inputs (if any)
.OUTPUTS
    Output (if any)
.NOTES
    General notes
#>
    [CmdletBinding(SupportsShouldProcess = $true)]
    Param(
        [Parameter(Position = 1, Mandatory = $true, ValueFromPipeline =$true,ValueFromPipelineByPropertyName = $True)]
        [string[]] $Projects,
        
        [Parameter(Position = 2)]
        [string[]] $Configuration = $null,
        
        [string] $DestinationPath = $null,
        
        [string] $RunSettings = $null,

        [String] $TestConsolePath,
        
        [string] $VisualStudioVersion = "15",
        
        [int] $MaxCpuCount = 6,
        
        [string] $TestAssemblyPattern = $null,
        
        [string] $TestAdapterPath,

        [Switch] $Redirect,
        
        [string] $Framework,
        
        [ScriptBlock] $Pass,
        
        [ScriptBlock] $Fail
    )

    $vsTest = $TestConsolePath
    if([string]::IsNullOrWhiteSpace($TestConsolePath) -or ! (Test-Path $vsTest)) {
        $vsTest = Get-GzVisualStudioTestConsolePath 
    }
   
    if([string]::IsNullOrWhiteSpace($vsTest) -or !(Test-Path $vsTest)) {
        Write-Error "Could not located VsTest.Console.exe, please use the -TestConsolePath"
        return @(1);
    }

    if(!$Configuration -or $Configuration.Length -eq 0) {
        $Configuration = @("Release")
    }

  

    if([string]::IsNullOrWhiteSpace($TestAssemblyPattern)) {
        $TestAssemblyPattern = "**\*Test*.dll"
    }

    $runSettingsIsFile = $false
    $hasRunSettings = ![string]::IsNullOrWhiteSpace($RunSettings);
 
    if($hasRunSettings -and $DestinationPath) {
        if($RunSettings.Contains("<?xml")) {
            $doc = [xml]$RunSettings
        } else {
            $data = [IO.File]::ReadAllText($RunSettings)
            $doc = [xml]$data
            $runSettingsIsFile = $true 
        }
        
        $doc.RunSettings.RunConfiguration.ResultsDirectory.Value = $DestinationPath
        if($runSettingsIsFile) {
            $doc.Save($RunSettings);
        } else {
            $RunSettings = $doc.ToString()
        }
    }

    if($DestinationPath -and !$hasRunSettings) {
       
           
            $RunSettings =  "<?xml version=`"1.0`" encoding=`"utf-8`"?>  
<RunSettings>  
<RunConfiguration>
<MaxCpuCount>$MaxCpuCount</MaxCpuCount>
<!-- Path relative to solution directory -->  
<ResultsDirectory>$DestinationPath</ResultsDirectory>  
</RunConfiguration>
</RunSettings>"
    }

    $results = @();

    foreach($project in $Projects) {
        $invoked = $false;
        $project = $project.Replace("/", "\");
       
        foreach($buildConfiguration in $Configuration) {
            if(!$project.EndsWith(".dll")) {
                $testAssembly = [IO.Path]::Combine($project,"bin", $buildConfiguration, $TestAssemblyPattern);
                $testAssembly = $testAssembly.Replace("/", "\")
              
                $testAssembly = Get-Item "$testAssembly" 
            } else {
                if($invoked) {
                    continue;
                }
                $testAssembly = Get-Item $project 
                $invoked = $true
            }

            if($testAssembly -is [Array])
            {
                $t = $null
                foreach($next in $testAssembly)
                {
                   
                    if($next.Name -match "xunit\." -or $next.Name -match "mstest\.") {
                        
                        continue;
                    } else {
                        $t = $next.FullName 
                        break;
                    }
                     
                }
               
                $testAssembly = $t;
            }

            if(!$testAssembly) {
                Write-Warning "Test assembly for $project could not be found: $testAssembly"
                $results += 1;
                continue;
            }

            if($testAssembly.FullName) {
                $testAssembly = $testAssembly.FullName.Replace("\\", "\")
            }
           
            $assemblyName = [System.IO.Path]::GetFileNameWithoutExtension($testAssembly);
            
            if($testAssembly -match "netcoreapp") {
                $parentDir = Split-Path $testAssembly
                $index = $parentDir.IndexOf("netcoreapp")
                $version = $parentDir.Substring($index + 10)
                $Framework = ".NETCoreApp,Version=v$version"
            }
            $parameters = @(
                "$testAssembly",
                "/Logger:`"trx`""
            )
            if(![string]::IsNullOrWhiteSpace($TestAdapterPath)) {
                $parameters += "/TestAdapterPath:`"$TestAdapterPath`""
            }
            if(![string]::IsNullOrWhiteSpace($Framework)) {
                $parameters += "/framework:`"$Framework`""
            }
            if(! [string]::IsNullOrWhiteSpace($DestinationPath)) {
                                
                if((Test-Path "$DestinationPath\$assemblyName.trx")) {
                    Remove-Item "$DestinationPath\$assemblyName.trx"
                }
                
                if(!$hasRunSettings) {
                    $runSettingsPath = [IO.Path]::Combine($DestinationPath, "$assemblyName.RunSettings.xml")
                    if(Test-Path $runSettingsPath) {
                         Remove-Item $runSettingsPath 
                    }
                    if(!(Test-Path $DestinationPath)) {
                        New-Item $DestinationPath -ItemType Directory -Force | Write-Debug
                    }
                    $RunSettings | Out-File $runSettingsPath -Encoding "utf8"
                    $RunSettings = $runSettingsPath
                }
            } 

            if(![string]::IsNullOrWhiteSpace($RunSettings)) {
                $parameters += "/Settings:`"$RunSettings`"" 
            }

            if($PSCmdlet.ShouldProcess("$vsTest $([string]::Join(' ', $parameters))")) {
                    if($Redirect.ToBool()) {
                        #TODO: invoke as a process and redirect to sb.
                        $out = & $vsTest @parameters 
                        $sb = new-Object System.Text.StringBuilder
                        [void]$sb.Append($out)
                        
                        $results += [PSCustomObject]@{
                            ExitCode = $LASTEXITCODE
                            Out = $sb
                        }
                        continue;
                    }
                    & $vsTest @parameters | Out-Host
                    $results += $LASTEXITCODE

                    if($LASTEXITCODE -ne 0) {
                        if($Fail) {
                            $Fail.Invoke($testAssembly, $parameters)
                        } else {
                            Write-Host "--------------------------------------------"
                            Write-Error "$assemblyName failed"
                            Write-Host ""
                            Write-Host ""
                        }
                      
                    } else {
                        if($Pass) {
                            $Pass.Invoke($testAssembly, $parameters)
                        } else { 
                            Write-Host "--------------------------------------------"
                            Write-Host "$assemblyName passed" -ForegroundColor "Green"
                            Write-Host ""
                            Write-Host ""
                        }
                    }
            } else {
                $results += 0
            }
        }
    }

    return $results;
}
