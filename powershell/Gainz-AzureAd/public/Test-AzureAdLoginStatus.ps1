
$isAzureAdConnected = $null
function Test-AzureAdLoginStatus() {

    Param(
        [Switch] $Login
    )

    if($null -eq $isAzureAdConnected)
    {
        $isAzureAdConnected = $false;

        Try 
        {
            $content = Get-AzureRmContext
            if ($content) 
            {
                $isAzureAdConnected = !([string]::IsNullOrEmpty($content.Account))
            } 
        } 
        Catch 
        {
            if ($_ -like "*Login-AzureRmAccount to login*") 
            {
                $isAzureAdConnected = $false;
            } 
            else 
            {
                throw
            }
        }
    }

    if(!$isAzureAdConnected -and $Login.ToBool()) {
        Connect-AzureAD 
        $isAzureAdConnected = $true;
    }

    return $isAzureAdConnected;
}