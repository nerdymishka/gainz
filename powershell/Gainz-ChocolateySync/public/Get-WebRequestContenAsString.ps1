
function Get-WebRequestContentAsString() {
    Param(
        [Parameter(Position = 0, ValueFromPipeline = $true)]
        [String] $Uri 
    )

    $client = New-WebClient -Uri $Uri
    return $client.DownloadString($Uri)
}