if(!$PSScriptRoot) {
    $PSScriptRoot = $MyInovocation.PSScriptRoot
}

if([Type]::GetType("HtmlAgilityPack.HtmlDocument") -eq $Null) {
    if($PSVersionTable.PSEdition -eq "Core") {
        [System.Reflection.Assembly]::LoadFile("$PSScriptRoot\bin\netstandard2.0\HtmlAgilityPack.dll") | Out-Null
    } else {
        [System.Reflection.Assembly]::LoadFile("$PSScriptRoot\bin\net45\HtmlAgilityPack.dll") | Out-Null
    }
}

function New-HtmlDocument() {
    Param(
        [String] $Path,
        [String] $Content 
    )

    if([string]::IsNullOrWhiteSpace($Path) -and [string]::IsNullOrWhiteSpace($Content)) {
        throw [System.ArgumentException] "Path or Content must be specified"
    }

    if($Path -and ![string]::IsNullOrWhiteSpace($Path)) {
        if(!(Test-Path $Path)) {
            throw [System.IO.FileNotFoundException] $Path 
        }

        $Content = Get-Context $Path -Raw 
    }

    $Html = New-Object HtmlAgilityPack.HtmlDocument 
    $Html.LoadHtml($Content)

    return $Html
}

function Select-Html () {
    Param(
        [Parameter(Position = 0, ValueFromPipeline = $true)]
        [String] $Content,

       
        [HtmlAgilityPack.HtmlNode] $Html,

        [String] $Path,
        
        [String] $XPath,
        
        [Switch] $NodesOnly
    )

    $p = $null;
    if($Path -and ![string]::IsNullOrWhiteSpace($Path)) {
        if(!(Test-Path $Path)) {
            throw [System.IO.FileNotFoundException] $Path 
        }

        $p = $Path;
        $Content = Get-Context $Path -Raw 
    }

    $Node = $null
    if($Content -and ![string]::IsNullOrWhiteSpace($Content)) {
        $Doc = New-Object HtmlAgilityPack.HtmlDocument 
        $Doc.LoadHtml($Content)
        $Node = $Doc.DocumentNode 
        write-host "setting node"

                
    }

    if($Html -ne $null) {
        Write-Host "setting node to html"
        $Node = $Html
    }

  
    if($Node -eq $null) {
        throw [ArgumentException] "Html, Content, or Path should be specified"
    }
   
    $nodes = $Node.SelectNodes($XPath)

    if($NodesOnly.ToBool()) {
        return $nodes;
    }

    $results = @()
    foreach($node in $nodes) {

        $result = New-Object PsCustomObject -Property @{
            Node = $node
            Path = $p
            Pattern = $XPath
        }

        $results += $result 
    }

    $results 
}