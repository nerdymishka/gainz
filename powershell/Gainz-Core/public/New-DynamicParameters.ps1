function New-DynamicParameter() {
    Param(
        [Parameter(Position = 0, Mandatory = $true)]
        [String] $Name,

        [Parameter(Position = 1)]
        [Alias("i")]
        [Nullable[Int]] $Position = $null,

        [Parameter(Position = 2)]
        [Alias("t")]
        [Type] $Type = [String],

        [Parameter(Position = 3)]
        [Alias("Set")]
        [String] $SetName = "__AllParameterSets",

        [Alias("r")]
        [Switch] $Required,
        
        [Alias("p")]
        [Switch] $FromPipeline,
        
        [Alias("pp")]
        [Switch] $FromPipelineProperty,
        
        [Switch] $FromRemainingArgs,
        
        [Alias("m")]
        [String] $HelpMessage,
        
        [Alias("a")]
        [String[]] $Alias = $null,
        
        [Alias("m")]
        [Alias("vs")]
        [Alias("tab")]
        [String[]] $ValidationSet,

        [Pararmeter(ValueFromPipeline = $true)]
        [System.Management.Automation.RuntimeDefinedParameterDictionary] $Parameters
    )

    $col = New-Object 'Collections.ObjectModel.Collection[System.Attribute]'
    $paramAttr = New-Object  System.Management.Automation.ParameterAttribute
    if($Position.HasValue) {
        $paramAttr.Position = $Position.Value;
    }

    if($Required.ToBool()) {
        $paramAttr.Mandatory = $true;
    }

    if(![string]::IsNullOrWhiteSpace($HelpMessage)) {
        $paramAttr.HelpMessage = $HelpMessage
    }

    if($FromPipeline.ToBool()) {
        $paramAttr.ValueFromPipeline = $true;
    }

    if($FromPipelineProperty.ToBool()) {
        $paramAttr.ValueFromPipelinePropertyName = $true;
    }

    if($FromRemainingArgs.ToBool()) {
        $paramAttr.ValueFromFromRemaingArguments = $true;
    }

    $col.Add($paramAttr)

    if($null -ne $Alias -and $Alias.Length -gt 0) {
        $aliasAttr = New-Item  System.Management.Automation.AliasAttribute -ArgumentList $Alias
        $col.Add($aliasAttr)
    }

    if($null -ne $ValidationSet -and $ValidationSet.Length -gt 0) {
        $set = New-Item System.Management.Automation.ValidateSetAttribute -ArgumentList $ValidationSet
        $col.Add($set)
    }

    $Parameter = New-Object System.Management.Automation.RuntimeDefinedParameter -ArgumentList @(
        $Name, 
        $Type, 
        $col)

    if($Parameters) {
        $Parameters.Add($Name, $Parameter);
    } else {
        return $Parameter;
    }
}