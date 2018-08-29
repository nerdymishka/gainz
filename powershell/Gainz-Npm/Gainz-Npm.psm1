
function Invoke-Npm() {
    Param(
        [Parameter(Position = 0, ValueFromRemainingArguments)]
        [String] $Arguments,

        [Switch] $Redirect,

        [Alias("wd")]
        [String] $WorkingDirectory,

        [Alias("out")]
        [Ref] $StandardOutput,

        [Alias("error")]
        [Ref] $ErrorOutput 
    )

    $program = $null
    if($Env:OS -eq "Windows_NT") {
        $program = "npm.cmd"
    } else {
        $program = "npm.sh"
    }

    if([string]::IsNullOrWhiteSpace($WorkingDirectory)) {
        $WorkingDirectory = (Get-Location).Path
    }

    $p = Get-Command $program -ErrorAction SilentlyContinue
    if(!p) {
        Write-Error "NPM was not found on the path"
    }

    $p = $p.Path 
  
    $si = New-Object System.Diagnostics.ProcessStartInfo 
    $si.Arguments = $Arguments
    $si.FileName = $p
    $si.RedirectStandardOutput = $true 
    $si.RedirectStandardError = $true 
    $si.UseShellExecute = $false 
    $si.WorkingDirectory = $WorkingDirectory

    $proc = New-Object System.Diagnostics.Process
    $proc.StartInfo = $si 
  
    $out = New-Object System.Collections.Generic.List[System.String]
    $error = New-Object System.Collections.Generic.List[System.String] 

    if($Redirect.ToBool()) {
        if($WriteOutput -eq $null) {
            $WriteOutput = {
                Param(
                    [Parameter(Position = 0)]
                    [String] $Line 
                )
                Write-Host "test"
                Write-Host $Line
            }
        } 

        if(!$WriteError -eq $Null) {
            $WriteError = {
                Param(
                    [Parameter(Position = 0)]
                    [String] $Line 
                )
                Write-Error $Line
            }
        } 

        $ev1 = Register-ObjectEvent -InputObject $proc -EventName "OutputDataReceived" -Action {
            $Event.MessageData.Add($EventArgs.Data)
            Write-Host $EventArgs.Data
           
        } -MessageData $out 

        $ev2 = Register-ObjectEvent -InputObject $proc -EventName "ErrorDataReceived" -Action {
            $Event.MessageData.Add($EventArgs.Data)
            Write-Error ($EventArgs.Data) 
        } -MessageData $error 
    } else {
        $ev1 = Register-ObjectEvent -InputObject $proc -EventName "OutputDataReceived" -Action {
            $Event.MessageData.Add($EventArgs.Data)
           
        } -MessageData $out 

        $ev2 = Register-ObjectEvent -InputObject $proc -EventName "ErrorDataReceived" -Action {
            $Event.MessageData.Add($EventArgs.Data)
           
        } -MessageData $error 
    }
   
    
    $proc.Start() | Out-Null 
    $proc.BeginOutputReadLine() | Out-Null 
    $proc.BeginErrorReadLine() | Out-Null
    $proc.WaitForExit()

    Unregister-Event $ev1.Id 
    Unregister-Event $ev2.Id 

    if($StandardOutput) {
        if($StandardOutput -is [String]) {
            $StandardOutput = [String]::Join($out.ToArray())
        } else {
            $StandardOutput = $out.ToArray()
        }
    }

    if($ErrorOutput) {
        if($ErrorOutput -is [String]) {
            $ErrorOutput = [String]::Join($error.ToArray())
        } else {
            $ErrorOutput = $error.ToArray()
        }
    }

    return $proc.ExitCode
}

function Invoke-NpmScript() {
    Param(
        [Parameter(Position = 0)]
        [String] $Script,

        [Parameter(ValueFromRemainingArguments = $true)]
        [String] $Arguments,

        [Alias("wd")]
        [String] $WorkingDirectory,

        [Alias("out")]
        [Ref] $StandardOutput,

        [Alias("error")]
        [Ref] $ErrorOutput
    )

    Invoke-NpmScript "$Script $Arguments" -wd $WorkingDirectory `
        -out $StandardOutput -Error $ErrorOutput
}

function Install-NpmPackage() {
    Param(
        [Parameter(Position = 0)]
        [String[]] $Packages,

        [Switch] $Redirect,

        [Alias("wd")]
        [String] $WorkingDirectory,

        [Alias("out")]
        [Ref] $StandardOutput,

        [Alias("error")]
        [Ref] $ErrorOutput,

        [Alias("D")]
        [Switch] $SaveDev, 

        [Alias("P")]
        [Switch] $SaveProd,
        
        [Switch] $NoSave,

        [Alias("O")]
        [Switch] $SaveOptional,

        [Alias("E")]
        [Switch] $SaveExact,

        [Alias("B")]
        [Switch] $SaveBundle,

        [Switch] $WhatIf 
    )

    $cmd = "install"

    if($Packages -and $Packages.Length) {
        $cmd += " " + [String]::Join(" ", $Packages)
    }

    if($SaveDev.ToBool()) {
        $cmd += " --save-dev"
    }

    if($Save.ToBool()) {
        $cmd += " --save"
    }

    if($SaveProd.ToBool()) {
        $cmd += " --save-prod"
    }

    if($SaveExact.ToBool()) {
        $cmd += " --save-exact"
    }

    if($SaveOptional.ToBool()) {
        $cmd += " --save-optional"
    }

    if($SaveBundle.ToBool()) {
        $cmd += " --save-bundle"
    }

    if($WhatIf.ToBool()) {
        $cmd += " --dry-run"
    }

    Invoke-Npm $cmd -Redirect:$Redirect -WorkingDirectory `
        $WorkingDirectory -out $StandardOutput -ErrorOutput $ErrorOutput
}

Export-ModuleMember @(
    "Invoke-Npm",
    "Install-NpmPackage",
    "Invoke-NpmScript"
)