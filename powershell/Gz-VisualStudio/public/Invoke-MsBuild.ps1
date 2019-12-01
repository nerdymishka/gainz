function Invoke-MsBuild() {
<#
.SYNOPSIS
    Invoke Microsoft Build

.DESCRIPTION
    Invokes the msbuild executable using System.Diagnostics.Process
    with the supplied parameters. The command will redirect the output
    to the host and returns the exit code.

.PARAMETER InputObject
    The project that msbuild should build. This can be a solution file
    project file. Aliases: -Project, -Path

.PARAMETER Targets
    Optional. The targets to invoke in a semi colon delimited string e.g.
    Clean;Build. Aliases: -T

.PARAMETER LogFile
    Optional. The path to the location where the logfile should be saved. 

.PARAMETER MsBuildPath
    Optional. The path to the location for the version of MsBuild that
    should be used. If this isn't supplied this cmdlet will attempt to 
    find the most recent msbuild build on the current system or use
    the environment variable $Env:FMG_MSBUILD. 

.PARAMETER ArgumentList
    Optional. The list of arguments that should be passed to msbuild.
    Aliases: -Args

.PARAMETER TimeOut
    Optional. The time in miliseconds the process will wait before
    execution is aborted. 

.EXAMPLE
    PS C:\> $result = Invoke-MsBuild "$Home/Projects/Project.sln"
    Invokes msbuild for the Project.sln

.INPUTS
    The solution or project file.

.OUTPUTS
    Returns the last exit code for msbuild.

#>
    [CmdletBinding()]
    Param(
        [Alias("Path")]
        [Alias("Project")]
        [Parameter(Position = 0, Mandatory = $true)]
        [string] $InputObject, 

        [Alias("T")]
        [string] $Targets,
        
        [string] $LogFile,
        
        [string] $MsBuildPath,

        [Switch] $Redirect,
        
        [Alias("Args")]
        [Parameter()]
        [string[]] $ArgumentList,
        
        [Nullable[int]] $TimeOut
    )

    if([string]::IsNullOrWhiteSpace($MsBuildPath)) {
        if($Env:GZ_MSBUILD_PATH) {
            $MsBuildPath = $Env:GZ_MSBUILD_PATH
        }  else {
            $MsBuildPath = Get-MsBuildPath -Latest
        }
    }

    if(-not (Test-Path $MsBuildPath)) {
        Throw "Could not locate MsBuild at $MsBuildPath"
    }

    if(-not (Test-Path $InputObject)) {
        Throw [System.IO.FileNotFoundException] "Could not find Project or Solution: $InputObject"
    }

    $argz = @("`"$InputObject`"",
        '/nologo',
        '/nr:false'
    )

    if($Targets) {
        $ts = $Targets.Split(";")
        foreach ($t in $ts) {
            $argz += "/t:$t"
        }
    }

    if(![string]::IsNullOrWhiteSpace($ArgumentList)) {
        $argz += "$ArgumentList"   
    }

    if($LogFile) {
        $argz += "\/fl \/flp:`"logfile=$LogFile`""
    }


    $ag = [string]::Join(" ", $argz);
    $result = 0;
    $p = $null;
    $info = New-Object System.Diagnostics.ProcessStartInfo
    $info.FileName = $MsBuildPath


    $info.RedirectStandardError = $false 
    $info.RedirectStandardOutput = $false 
    
    $info.UseShellExecute = $false
    $info.Arguments = $ag 
    $p = New-Object System.Diagnostics.Process
    $p.StartInfo = $info

    $stdOutEvent = $null;
    $stdErrEvent = $null
    
    if($Redirect.ToBool()) {
        $stdOutBuilder = New-Object -TypeName System.Text.StringBuilder
        $stdErrBuilder = New-Object -TypeName System.Text.StringBuilder
        $info.RedirectStandardError = $true 
        $info.RedirectStandardOutput = $true
        $action = {
            if (! [String]::IsNullOrEmpty($EventArgs.Data)) {
                $Event.MessageData.AppendLine($EventArgs.Data)
            }
        }
        $stdOutEvent = Register-ObjectEvent -InputObject $p `
            -Action $action -EventName 'OutputDataReceived' `
            -MessageData $stdOutBuilder
        $stdErrEvent = Register-ObjectEvent -InputObject $p `
            -Action $action -EventName 'ErrorDataReceived' `
            -MessageData $stdErrBuilder

        [void]$p.Start();
        $p.BeginOutputReadLine();
        $p.BeginErrorReadLine();
        if($TimeOut.HasValue -and $TimeOut.Value -gt 10) {
            [void]$p.WaitForExit($TimeOut.Value)
        } else {
            [void]$p.WaitForExit()
        }


        Unregister-Event -SourceIdentifier $stdOutEvent.Name
        Unregister-Event -SourceIdentifier $stdErrEvent.Name

        $p.Dispose();
        return [PsCustomObject]@{
            ExitCode = $result 
            Out = $stdOutBuilder
            Error = $stdErrBuilder
        }

    } else {
        [void]$p.Start()
        if($TimeOut.HasValue -and $TimeOut.Value -gt 10) {
            [void]$p.WaitForExit($TimeOut.Value)
        } else {
            [void]$p.WaitForExit()
        }

        $result = $p.ExitCode
        $p.Dispose()
        return $result
    }

}