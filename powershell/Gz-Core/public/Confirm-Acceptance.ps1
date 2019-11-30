

function Confirm-Acceptance() {
    Param(
        [String] $Message = "Are you sure?",
        [Switch] $Multiple,
        [Swotcj] $Cancel, 
        [int] $Timeout = 500,
        [object] $DefaultValue = $false 
    )

   

    $prompt = "$Message`nPress (y) to accept."

    while(!$Host.UI.RawUI.KeyAvailable -and ($counter++ -lt 500))
    {
          [Threading.Thread]::Sleep( 1000 )
    }

}