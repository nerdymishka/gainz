
function Add-DynamicParameter() {
    Param(
        [Parameter(ValueFromPipeline = $True, ValueFromPipelineByPropertyName = $True)]
        [System.Management.Automation.RuntimeDefinedParameterDictionary] $Dictionary,

        [Parameter(Position = 0)]
        [System.Management.Automation.RuntimeDefinedParameter] $Parameter,

        [Switch] $PassThru 
    )

    if($PassThru.ToBool()) {
        $Dictionary = New-Object System.Management.Automation.RuntimeDefinedParameterDictionary
        [void]$Dictionary.Add($Parameter.Name, $Parameter);
        return $Dictionary;
    }

    [void]$Dictionary.Add($Parameter.Name, $Parameter);
}