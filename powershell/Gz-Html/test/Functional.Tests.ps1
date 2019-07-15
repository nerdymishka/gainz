Test-ModuleManifest "$PsScriptRoot/../Gz-Html.psd1"
Import-Module "$PsScriptRoot/../Gz-Html.psd1" -Force 

Describe "Gz-Html" {
    Context "Select-GzHtml" {
       IT "Should select nodes from a WebRequest" {
            $downloadUri = "http://www.7-zip.org/download.html"
            $content = Invoke-WebRequest $downloadUri -UseBasicParsing
            $content | Should Not Be $Null
    
            $node = $content | Select-Html -XPath "/html/body/table/tr/td[2]/p[1]" -NodesOnly
            $node | Should Not Be $null
            $node.InnerText | Should Not Be $null 
            $node.InnerText.Contains(".") | Should Be $true 
            Write-Host $node.InnertText 
       }

       It "Should node from content." {
           $node = "<html><body><h1>Hi!</h1></body></html>" | Select-Html -XPath "/html/body/h1" -NodesOnly
           $node | Should Not Be $null
           $node.InnerText | Should Be "Hi!"
       }
    }
}