function Save-WebRequestContentAsFile() {
    Param(
        [Parameter(Position = 0, ValueFromPipeline = $True)]
        [String] $Uri,

        [Parameter(Position = 1)]
        [String] $Destination 
    )

    $client = New-WebClient -Uri $Uri
    $client.DownloadFile($Uri, $Destination)
}