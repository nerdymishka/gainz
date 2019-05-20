function ConvertTo-Hashtable() {
    [CmdletBinding()]
    Param(
        [Parameter(Position = 0, ValueFromPipeline = $true, ValueFromPipelineByPropertyName = $true)]
        [Object] $InputObject 
    )

    PROCESS
    {
        if ($null -eq $InputObject) { return $null }

        if ($InputObject -is [System.Collections.IEnumerable] -and $InputObject -isnot [string])
        {
            $collection = @(
                foreach ($object in $InputObject) { 
                    ConvertTo-Hashtable $object 
                }
            )

            Write-Output -NoEnumerate $collection
        }
        elseif ($InputObject -is [psobject])
        {
            $hash = @{}

            foreach ($property in $InputObject.PSObject.Properties)
            {
                $hash[$property.Name] = ConvertTo-Hashtable $property.Value
            }

            return $hash
        }
        else
        {
            return $InputObject
        }
    }
}