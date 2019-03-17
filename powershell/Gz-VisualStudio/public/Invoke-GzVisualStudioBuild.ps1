Add-Type -TypeDefinition @"
    public enum VisualStudioBuildSwitches {
        Build,
        Clean,
        Deploy,
        Rebuild
    }
"@

function Invoke-GzVisualStudioBuild() {

<#
.SYNOPSIS
    Invokes the msbuild that is tied to a specific version 
    of Visual Studio.

.DESCRIPTION
    Visual Studio comes with a bundled version of msbuild. In
    the newer version the rosalyn compiler is part of the 
    custom versionsof msbuild. 
    
    This cmdlet will find the version of msbuild associated
    with a particular version of Visual Studio and invoke it.  
    
    Specifing multiple configurations and platforms will 
    create a matrix of invokes that will run msbuild for 
    all the supplied configurations and platforms and will
    return the exit codes. 

.PARAMETER InputObject
    Required. The project or solution that will be built. 
    Aliases: -Path, -Project, -Solution

.PARAMETER Targets
    Optional. The targets that will be invoked. This is a 
    semi-colon delimited string e.g. "Clean;Build"
        
.PARAMETER Configuration
    Optional. The target build configuration(s) that should be used.
    This defaults to "Release". Aliases: -C
        
.PARAMETER Platform
    Optional. The target platform(s) that should be used. The default
    is "Any CPU". Aliases: -P
        
.PARAMETER ArgumentList
    Optional. The arguments that should be passed to msbuild.
    Aliases: -Args

.PARAMETER Parallel
    Optional. This switch instructs msbuild to use parallel builds.
    Aliases: -M
    
.PARAMETER NugetRestore
    Optional. This switch instructs runs a nuget restore. If Visual
    Studio is less than version 15.0, it uses the built in nuget restore.
    For version 15.0 the cmdlet will attempt to find the nuget on the path
    or use the environment variable $Env:GZ_NUGET_PATH. Aliases: -Restore

.PARAMETER Redirect
    Optional. This switch instructs the output to be captured in a string
    builder and returned as part of the results. 

.PARAMETER VisualStudioVersion
    Optional. The version of Visual Studio to use. By default this
    cmdlet will use the latest version of Visual Studio. Aliases: -Version
        
.EXAMPLE
    PS C:\> $results = Invoke-VisualStudioBuild "$Home/Project/Project.sln"
    
.OUTPUTS
    Returns an array of last exit codes or an array of custom powershell objects
    that includes the ExitCode,Out,Error properties. Out & Error are string builders
    ouf the output. 

#>
    # TODO: add logfile variable
    # TODO: add timeout variable

    [CmdletBinding(SupportsShouldProcess = $True)]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $True)]
        [Alias("Path")]
        [Alias("Solution")]
        [Alias("Project")]
        [string] $InputObject,

        [Parameter(Position = 1)]
        [String] $Targets = "Clean;Build",
        
        [Alias("C")]
        [Parameter(Position = 2)]
        [string[]] $Configuration = $null,
        
        [Alias("P")]
        [string[]] $Platform = $null,
        
        [Alias("Args")]
        [string[]] $ArgumentList = $null,

        [Alias("M")]
        [Switch] $Parallel,

        [Alias("Restore")]
        [Switch] $NugetRestore,

        [Switch] $Redirect,
        
        [Alias("Version")]
        [string] $VisualStudioVersion = $null
    )

    if(!(Test-Path $InputObject)) {
        throw [System.IO.FileNotFoundException] $InputObject
    }

    $Project = (Resolve-Path $InputObject).Path

    if([string]::IsNullOrWhiteSpace($VisualStudioVersion)) {
        $VisualStudioVersion = "latest"
    }

    if($null -eq $Configuration -or $Configuration.Length -eq 0) {
        $Configuration = @("Release")
    }

    if($null -eq $Platform -or $Platform.Length -eq 0) {
        $Platform = @("Any%20CPU")
    }

    if($null -eq $ArgumentList) {
        $ArgumentList = @()
    }

    $vsVersion = Get-GzVisualStudioVersion $VisualStudioVersion
    $msBuildPath = Get-GzMsBuildPath -Version $vsVersion;
    
   
    if(! (Test-Path $msBuildPath)) {
        Write-Error "Unable to find msbuild at $msBuildPath"
    }
    
    $results = @();
    $argz = $ArgumentList

    if($NugetRestore.ToBool()) {
        Write-Debug "Restoring Packages.."
        if($vsVersion -ne "15.0") {
            $nugetExe = Get-Command nuget -ErrorAction SilentlyContinue
            if($nugetExe) {
                $nugetExe = $nugetExe.Path
            }

            if($Env:GZ_NUGET_PATH) {
                $nugetExe = $Env:GZ_NUGET_PATH
            } 
            if(Test-Path $nugetExe) {
                $p = (Start-Process $nugetExe -ArgumentList "restore $Project" -NoNewWindow -PassThru -Wait)
                
                if($p.ExitCode -ne 0) {
                    Write-Error "Nuget Restore Failed for $Project"
                }
            } else {
                Write-Warning "Nuget not found: $nugetExe"
            }
        } else {
            $Targets = "Restore;$Targets"
        }
    } 

    foreach($buildConfiguration in $Configuration) {
        foreach($buildPlatform in $Platform) {
            
            $argz += "/p:Configuration=`"${buildConfiguration}`""
            $argz += "/p:Platform=`"${buildPlatform}`""    
            $argz += "/p:VisualStudioVersion=`"$vsVersion`""
            
            if($Parallel.ToBool()) {
                $argz += "/m"
            }
           
            $message = "$msBuildPath /t:$Targets $argz"
           
            if($PSCmdlet.ShouldProcess($message)) {
                $results += Invoke-GzMsBuild -Path $Project -Targets $Targets -ArgumentList $argz -MsBuildPath $msBuildPath -Redirect:$Redirect
            } else {
                $results += 0
            }
        }
    }
    
    return $results 
}