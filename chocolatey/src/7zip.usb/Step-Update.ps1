$mods = "$PSScriptRoot/../Modules"
$root = "$PSScriptRoot/../.."

Import-Module "$mods/Fmg-Html" -Force
Import-Module "$mods/Fmg-ResolveStache" -Force  

$artifacts = $Global:Artifacts
if(!$artifacts) {
    $artifacts = "$root/build/artifacts"
}

$artifacts = "$artifacts/installers/7zip"
if(!(Test-Path $artifacts)) {
    New-Item $artifacts -ItemType Directory -Force
}


function Select-Version() {
    Param(
        [Parameter(Position = 0)]
        [String] $Text 
    )

    $parts = $Text.Split(' ')
    $version = $null;
    for($i = 0; $i -lt $parts.Length; $i++) {
        $part = $parts[$i].Trim();
       
        $sb = New-Object System.Text.StringBuilder 
        $finished = $true;
        for($j = 0; $j -lt $part.Length; $j++) {
            $char = $part[$j]
            if([Char]::IsDigit($char) -or $char -eq '.') {
                $sb.Append($char) | Out-Null;
                continue;
            }
            $finished = $false;
            $sb.Clear() | Out-Null
            break;
        }

        if($finished) {
            $version = $sb.ToString();
            $sb.Clear() | Out-Null
            break;
        }
    }

    return $version
}

function Select-Urls() {
    Param(
        [Parameter(Position = 0)]
        [String] $Content 
    )

    $table = $Content | Select-Html -XPath "/html/body/table/tr/td[2]/table[1]" -NodesOnly
    
    $32bitUri
    $64bitUri
    foreach($row in $table.ChildNodes) {
       
        foreach($child in $row.ChildNodes) {
            
            if($child.Name -eq "td") {
                if($child.InnerText -match "Download") {
                    $tdLastDownload = $child              
                    continue;
                } 

                if($child.InnerText.Contains("MSI")) {
                    if($child.InnerText.Contains("32-bit")) {
                        $td32bit = $tdLastDownload
                        $a = $td32bit.ChildNodes[0];
                        if($a) {
                            $32bitUri = $a.Attributes["Href"].Value;
                        }
                       
                    }
                    if($child.InnerText.Contains("64-bit")) {
                        $td64bit = $tdLastDownload
                        $a = $td64bit.ChildNodes[0];
                        if($a) {
                            $64bitUri = $a.Attributes["Href"].Value;
                        }
                    }
                }
            }
        }
    }

    return New-Object PsCustomObject -Property @{
        "uri" = "http://www.7-zip.org/$32bitUri"
        "uri64" = "http://www.7-zip.org/$64bitUri"
    }
}

function Step-Update() {

    
    # 7-zip only supports http
    $downloadUri = "http://www.7-zip.org/download.html"
    $content = Invoke-WebRequest $downloadUri -UseBasicParsing
    $node = $content | Select-Html -XPath "/html/body/table/tr/td[2]/p[1]" -NodesOnly
    
    $version = Select-Version -Text ($node.InnerText)
   
    if(!$version) {
        Throw "Version could not be parsed from 7zip's download page"
    }
    
    $db = Get-Content "$PSScriptRoot/kweh.versions.json" `
          -Encoding "UTF8" | ConvertFrom-Json 
        
    if($db -eq $null)
    {
        $db = New-Object PsCustomObject
    }
  
    if(!$db.$version) {
     
        $uris = Select-Urls $content 

        if(!(Test-Path "$artifacts/$version")) {
            New-Item "$artifacts/$version" -ItemType Directory
        }
        
        Invoke-WebRequest -Uri ($uris.uri) -OutFile "$artifacts/$version/7zip_x32.msi" -UseBasicParsing
        Invoke-WebRequest -Uri ($uris.uri64) -OutFile "$artifacts/$version/7zip_x64.msi" -UseBasicParsing

        $hash = (Get-FileHash "$artifacts/$version/7zip_x32.msi").Hash 
        $hash64 = (Get-FileHash "$artifacts/$version/7zip_x64.msi").Hash 

        $result = [PsCustomObject] @{
            version = $version 
            uri = $uris.uri
            hash = $hash 
            uri64 = $uris.uri64 
            hash64 = $hash64
        }
       

        $content = Get-Content "$PSScriptRoot/tpl.json" -Raw
        $content = Resolve-Stache -Template $content -Model $result
        $versionJson = $content | ConvertFrom-Json 
        $db | Add-Member -MemberType NoteProperty -Name $version -Value $versionJson 
        $content = $db | ConvertTo-json -Depth 10 
        $content | Out-File "$PSScriptRoot/kweh.versions.json" -Encoding "UTF8"

        Write-Host "Updated"
        return $true;
    }

    return $false
}

Step-Update