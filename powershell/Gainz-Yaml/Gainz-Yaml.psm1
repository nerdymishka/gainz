
if($null -eq  [Type]::GetType("YamlDotNet.RepresentationModel.YamlScalarNode")) {
    if($PSVersionTable.PSEdition -eq "Core") {
        [System.Reflection.Assembly]::LoadFile("$PSScriptRoot\bin\netstandard1.3\YamlDotNet.dll") | Out-Null
    } else {
        [System.Reflection.Assembly]::LoadFile("$PSScriptRoot\bin\net45\YamlDotNet.dll") | Out-Null
    }
}



$fmgBooleanYes = @("true", "yes", "on", "Y", "y")
$fmgBooleanNo = @("false", "no", "off", "N", "n")

function New-YamlStream() {
    [CmdletBinding()]
    Param(
        [Parameter(Position = 0, Mandatory = $true, ValueFromPipeline = $true)]
        [String] $InputObject
    )

    $sr = New-Object System.IO.StringReader $InputObject 
    $ymlStream = New-Object YamlDotNet.RepresentationModel.YamlStream 
    $ymlStream.Load($sr)
    $sr.Dispose()

    return $ymlStream;
}

function ConvertFrom-YamlMappingNode() {
    Param(
        [Parameter(Position = 0, ValueFromPipeline = $true)]
        [YamlDotNet.RepresentationModel.YamlMappingNode] $InputObject
    )

    if($null -eq $InputObject) {
        return $null;
    }

    $section = New-Object PsCustomObject
    foreach($key in $InputObject.Children.Keys) {
        $value = $InputObject.Children[$key]
        $value = ConvertFrom-YamlNode $value
        $section | Add-Member NoteProperty -Name $key -Value $value
    }

    return [PsCustomObject]$section;
}

function ConvertFrom-YamlSequenceNode() {
    Param(
        [Parameter(Position = 0, Mandatory = $true, ValueFromPipeline = $true)]
        [YamlDotNet.RepresentationModel.YamlSequenceNode] $InputObject
    )

    
    if($null -eq $InputObject) {
        return $null;
    }

    $array = @();
    foreach($child in $InputObject.Children) {
        $array += (ConvertFrom-YamlNode $child)
    }

    return ,$array
}

function ConvertFrom-YamlScalarNode() {
    Param(
        [Parameter(Position = 0, ValueFromPipeline = $True)]
        $InputObject 
    )

    if($null -eq $InputObject) {
        return $Null;
    }

    if($InputObject -is [YamlDotNet.RepresentationModel.YamlScalarNode]) {
        $value = $InputObject.Value;

        if($InputObject.Tag) {
            Write-Host ($InputObject.Tag)
            switch($InputObject.Tag) {
                'tag:yaml.org,2002:str' { return $value.ToString() }
                'tag:yaml.org,2002:int' { return [int]$value}
                'tag:yaml.org,2002:float' { return [double]$value }
                'tag:yaml.org,2002:timestamp' { return [datetime]$value}
                'tag:yaml.org,2002:binary' { return [System.Convert]::FromBase64String($Value)}
                'tag:yaml.org,2002:null' { return $Null }
            }
        }

        # TODO: speed up type testing and parsing.
        if([String]::IsNullOrWhiteSpace($value)) {
            return $value;
        }

        if($fmgBooleanYes.IndexOf($value) -gt -1) {
            return $true;
        }

        if($fmgBooleanNo.IndexOf($value) -gt -1) {
            return $false;
        }

        [System.Int32]$integer = 0; 
        if([Int32]::TryParse($value, [ref] $integer)) {
            return $integer;
        }

        [System.Int64]$long = 0; 
        if([Int64]::TryParse($value, [ref] $long)) {
            return $long;
        }

        [System.Double] $double = $null;
        if([Double]::TryParse($value, [ref] $double)) {
            return $double;
        }

        [System.DateTime] $datetime = [DateTime]::MinValue;
        if([datetime]::TryParse($value, [ref] $datetime)) {
            return $datetime;
        }

        return $value;
    }

    return $InputObject.ToString()
}

function ConvertFrom-YamlNode() {
    Param(
        [Parameter(Position = 0, Mandatory = $true, ValueFromPipeline = $true)]
        [Object] $InputObject
    )

    $typeName =$InputObject.GetType().FullName
    switch($typeName) {
        "YamlDotNet.RepresentationModel.YamlSequenceNode" { 
            return ConvertFrom-YamlSequenceNode $InputObject  
        }
        "YamlDotNet.RepresentationModel.YamlMappingNode" { 
            return ConvertFrom-YamlMappingNode $InputObject
        }
        "YamlDotNet.RepresentationModel.YamlScalarNode" { 
            return ConvertFrom-YamlScalarNode $InputObject
        }

    }
    return  $converter.Invoke($InputObject)
}

function ConvertTo-YamlSerializeable() {
    Param(
        [Object] $InputObject
    )

    if($InputObject -is [System.Collections.IDictionary]) {
        return ConvertTo-YamlMappingNodeDictionary $InputObject
    }

    if($InputObject -is [System.Management.Automation.PsObject]) {
        return ConvertTo-YamlMappingNodeDictionary $InputObject
    }

    if($InputObject -is [System.Management.Automation.PSCustomObject]) {
        return ConvertTo-YamlMappingNodeDictionary $InputObject
    }

    if($InputObject -is [System.Collections.IList]) {
        return ConvertTo-YamlSequence 
    }

    return $InputObject;
}

function ConvertTo-YamlMappingNodeDictionary() {
    Param(
        [Parameter(Position = 0, ValueFromPipeline = $true)]
        [Object] $InputObject
    )

    $dictionary = New-Object System.Collections.Generic.Dictionary[string,object]

   

    if($InputObject -is [System.Collections.IDictionary]) {
        foreach($key in $InputObject.Keys) {
            $dictionary.Add($key.ToString(), $InputObject[$key])
        }

        return $dictionary;
    }

    if($InputObject -is [System.Management.Automation.PsCustomObject]) {
        $InputObject | Get-Member -MemberType NoteProperty | ForEach-Object {
            $name = $_.Name 
            $value = $InputObject.$name 
            $dictionary.Add($name, $value)
        }
    }

    if($InputObject -is [System.Management.Automation.PsObject]) {
        $InputObject.PsObject.Properties | ForEach-Object {
            $key = $_.Name
            $value = $InputObject.$Name
            $dictionary.Add($key, $value)
        }
        return $dictionary;
    }

    return $dictionary;
}

function ConvertTo-YamlSequenceArray() {
    Param(
        [Parameter(Position = 0, ValueFromPipeline = $true)]
        [Object] $InputObject
    )

    $array = @();
    foreach($child in $InputObject) {
        $array += ConvertTo-YamlSerializeable $child 
    }
    
    return ,$array
}

function ConvertFrom-Yaml() {
    Param(
        [Parameter(Position = 0, Mandatory=$false, ValueFromPipeline=$true)]
        [string] $InputObject
    )

    if([string]::IsNullOrWhiteSpace($InputObject)) {
        return $Null;
    }

    $ymlStream = $InputObject | New-YamlStream 
    if($ymlStream.Count -lt 1) {
        return $null;
    }

    $documents = @();
    foreach($document in $ymlStream) {
        $documents += (ConvertFrom-YamlNode $document.RootNode)
    }

    return $documents
}

function ConvertTo-Yaml() {
    Param(
        [Parameter(Position = 0, Mandatory =$true, ValueFromPipeline = $true)]
        [Object] $InputObject
    )

    if($null -eq $InputObject) {
        return $Null;
    }
    $sw = New-Object System.IO.StringWriter 
    $serializer = New-Object YamlDotNet.Serialization.Serializer(0)
    Try {
        $coerced = ConvertTo-YamlSerializeable $InputObject      
        $serializer.Serialize($sw, $coerced)
        return $sw.ToString()
    } finally {
        $sw.Dispose()
    }
}


Export-ModuleMember -Function @(
    'ConvertTo-Yaml',
    'ConvertFrom-Yaml'
)
