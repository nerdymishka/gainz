
$cmd = Get-Command "powershell.exe" -ErrorAction SilentlyContinue 

if(!$cmd) {
    $cmd = Get-Command "pwsh.exe" -ErrorAction SilentlyContinue 
}

function Invoke-KwehElevatedPowershell() {
    Param(
        [String] $Command,
        [String] $Script,
        [String] $Arguments,
        [String] $Module,
        [String] $Powershell
    )

    

    if($cmd -eq $null -and [string]::IsNullOrWhiteSpace($Powershell)) {
        $cmd = Get-Command "powershell.exe" -ErrorAction SilentlyContinue 

        if(!$cmd) {
            $cmd = Get-Command "pwsh.exe" -ErrorAction SilentlyContinue 
        }

        if(!$cmd) {
            throw "Could not locate powershell.exe"
        }
    }

    if([String]::IsNullOrWhiteSpace($Script) -and [String]::IsNullOrWhiteSpace($Command)) {
        throw ArgumentException "Command or Script must be specified"
    }

    if($Script -and ! (Test-PAth $Script)) {
        throw System.IO.FileNotFoundException $Script 
    }

    if([string]::IsNullOrWhiteSpace($Module)) {
        $Module = "$Env:ChocolateyInstall/helpers/chocolateyInstaller.psm1"
    }
    
    $import = "& import-module -name '$Module' -Verbose:`$false | Out-Null;"

    $statements = $null;

    if($Script) {
        Write-Debug "Attempting to run elevated powershell for script $Script"
        $statements = ". `"$Script`" $Arguments"
    }

    if($Command) {
        Write-Debug "Attempting to run elevated powershell for Command"
        $statements = "$Command"
    }

   

    $block = @"
      `
#`$Env:ChocolateyEnvironmentDebug='false'
#`$Env:ChocolateyEnvironmentVerbose='false'
$import

try {
    `$progressPreference="SilentlyContinue"
    $statements
} catch {
    throw
}
"@

    $encoded = [Convert]::ToBase64String([System.Text.Encoding]::Unicode.GetBytes($block))
    $powershellArgs = "-NoLogo -NonInteractive -NoProfile -ExecutionPolicy Bypass -InputFormat Text -OutputFormat Text -EncodedCommand $encoded"

    $isElevated = Test-IsElevated 
    
    if(!$isElevated) {
        $process = Start-Process ($cmd.Path) -ArgumentList $powershellArgs -Verb "RunAs" -PassThru  `
        -WindowStyle "Hidden" 
    } else {
        $process = Start-Process ($cmd.Path) -ArgumentList  $powershellArgs -PassThru  `
        -WindowStyle "Hidden" 
    }


    $process.WaitForExit()
      # sometimes the process hasn't fully exited yet.
    for ($i=1; $i -le 15; $i++) {
        if ($process.HasExited) { 
            break;
        }
        Write-Debug "Waiting for process to exit - $loopCount/15 seconds";
        Start-Sleep 1;
    }
    
    $exitCode = $process.ExitCode
    $exitCode
}

Set-Alias -Name Invoke-ElevatedPowershell -Value Invoke-KwehElevatedPowershell