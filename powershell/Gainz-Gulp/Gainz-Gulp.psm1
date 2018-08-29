$fmgNode = $null;

function Invoke-Gulp() {
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
   

    if([string]::IsNullOrWhiteSpace($WorkingDirectory)) {
        $WorkingDirectory = (Get-Location).Path
    }

    if(! (Test-Path "$WorkingDirectory/node_modules/gulp/bin/gulp.js")) {
        throw [System.IO.FileNotFoundException] "$WorkingDirectory/node_modules/gulp/bin/gulp.js"
    } else {
        $gulp = "$WorkingDirectory/node_modules/gulp/bin/gulp.js"
    }

    if(!$fmgNode) {
        $fmgNode = Get-Command node -ErrorAction SilentlyContinue 
        if($fmgNode) {$fmgNode = $fmgNode.Path}
    }

    if(!$fmgNode) {
        throw [System.IO.FileNotFoundException] "Node was not found on path"
    }
    

    $Arguments = "$gulp $Arguments"
  
    $si = New-Object System.Diagnostics.ProcessStartInfo 
    $si.Arguments = $Arguments
    $si.FileName = $fmgNode
    $si.RedirectStandardOutput = $true 
    $si.RedirectStandardError = $true 
    $si.UseShellExecute = $false 
    $si.WorkingDirectory = $WorkingDirectory

    $proc = New-Object System.Diagnostics.Process
    $proc.StartInfo = $si 
  
    $out = New-Object System.Collections.Generic.List[System.String]
    $error = New-Object System.Collections.Generic.List[System.String] 

    if($Redirect.ToBool()) {

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

Export-ModuleMember @("Invoke-Gulp")