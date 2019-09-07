function New-HtmlDocument() {
    Param(
        [Parameter(Position = 0)]
        [String] $Path,

        [Parameter(ValueFromPipeline = $true)]
        [System.IO.FileInfo] $InputObject,

        [Parameter(ValueFromPipeline = $true)]
        [String] $Content 
    )

    if([string]::IsNullOrWhiteSpace($Path) -and [string]::IsNullOrWhiteSpace($Content)) {
        throw [System.ArgumentException] "Path or Content must be specified"
    }

    if($InputObject) {
        if(!$InputObject.Exists) {
            throw [System.IO.FileNotFoundException] $InputObject.FullName
        }

        $Path = $InputObject.FullName;
    }

    if(![string]::IsNullOrWhiteSpace($Path)) {
        if(!(Test-Path $Path)) {
            throw [System.IO.FileNotFoundException] $Path 
        }

        $Content = Get-Content $Path -Raw 
    }

    $Html = New-Object HtmlAgilityPack.HtmlDocument 
    $Html.LoadHtml($Content)

    return $Html
}