

function Clear-ReadLineHistory() {
    [CmdletBinding(SupportsShouldProcess = $true)]
    Param(
        [ScriptBlock] $Where,
        [int] $Count = 0,
        [Switch] $All,
        [Switch] $Force 
    )

    Process {
        if($All.ToBool())
        {
            if($PSCmdlet.ShouldProcess("Should remove all command line history?")) {
                $execute = $Force.ToBool()
                if(!$execute) {
                    $execute = Read-Choice "Are you sure you want to remove all command line history?"
                    if($execute -match "yes") {
                        $execute = $true;
                    }
                }

                if($execute)
                {    
                    $historyPath = Get-PSReadLineOption | Select-Object -expand HistorySavePath 
                    Set-Content -Value "" -Force -Path $historyPath
                    return;
                }
            }
        }

        if($Where) {
            $historyPath = Get-PSReadLineOption | Select-Object -expand HistorySavePath 
            $content = $historyPath =Get-Content $historyPath 
            $set = @()
            foreach($line in $content) {
                if(!$Where.InvokeWithContext($line)) {
                    $set += $line
                }
            }

            Set-Content -Value $set -Force -Path $historyPath 
        }
    
        if($Count -gt 0)
        {
            $historyPath = Get-PSReadLineOption | Select-Object -expand HistorySavePath 
            $content = $historyPath =Get-Content $historyPath 

            $length = $content.Length - $count 
            if($length -lt 0) {
                Set-Content -Value "" -Force -Path $historyPath
                return 
            }
        }
    }
}
