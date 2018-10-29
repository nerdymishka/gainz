if(!$PSScriptRoot) {
    $PSScriptRoot = $MyInovocation.PSScriptRoot
}

if(![Type]::GetType("HtmlAgilityPack.HtmlDocument")) {
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

        [Parameter(ValueFromPipeline = $true)]
        [HtmlAgilityPack.HtmlNode] $Html,

        [Parameter(ValueFromPipeline = $true)]
        [Microsoft.PowerShell.Commands.BasicHtmlWebResponseObject] $Response,

        [String] $Path,
        
        [String] $XPath,
        
        [Switch] $NodesOnly
    )

    if($Response) {
        $Content = $Response.Content
    }

    $p = $null;
    if($Path -and ![string]::IsNullOrWhiteSpace($Path)) {
        if(!(Test-Path $Path)) {
            throw [System.IO.FileNotFoundException] $Path 
        }

        $p = $Path;
        $Content = Get-Content $Path -Raw 
    }

    $Node = $null
    if($Content -and ![string]::IsNullOrWhiteSpace($Content)) {
        $Doc = New-Object HtmlAgilityPack.HtmlDocument 
        $Doc.LoadHtml($Content)
        $Node = $Doc.DocumentNode 
    }

    if(!$Html) {  
        $Node = $Html
    }
  
    if(!$Node) {
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

Export-ModuleMember -Function @(
    'Select-Html',
    'New-HtmlDocument'
)