

function Clear-ReadLineHistory() {
    [CmdletBinding(SupportsShouldProcess = $true)]
    Param(
        [int] $Index = -1,
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
                    $execute = Read-Host "Are you sure? Press (y) to continue"
                    if($execute -match "y") {
                        $execute = $true;
                    }
                }


                $historyPath = Get-PSReadLineOption | Select-Object -expand HistorySavePath 
                Set-Content -Value "" -Force -Path $historyPath
                return;
            }
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