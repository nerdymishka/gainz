
function Select-Html() {
    [CmdletBinding()]
    Param(
        [Parameter(ValueFromPipeline = $true, ParameterSetName="Content")]
        [String] $Content,

        [Parameter(ValueFromPipeline = $true, ParameterSetName = "Html")]
        [HtmlAgilityPack.HtmlNode] $Html,

        [Parameter(ValueFromPipeline = $true, ParameterSetName = "Response")]
        [Microsoft.PowerShell.Commands.BasicHtmlWebResponseObject] $Response,

        [PArameter(ValueFromPipeline = $true, ParameterSetName = "File")]
        [System.IO.FileInfo] $InputObject,
        
        [Parameter(Mandatory = $true)]
        [String] $XPath,
        
        [Switch] $NodesOnly
    )

    PROCESS {

        if($Response) {
            $Content = $Response.Content
        }
        $Path = $null
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

        $Node = $null
        if(![string]::IsNullOrWhiteSpace($Content)) {
          
            $Doc = New-Object HtmlAgilityPack.HtmlDocument 
            $Doc.LoadHtml($Content)
            $Node = $Doc.DocumentNode        
        }

        if($Html) {  
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

        return $results 
    }
}