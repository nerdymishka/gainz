

function New-DynamicParameter() {
    Param(
        [Parameter(Position = 0, Mandatory = $true, ValueFromPipelineByPropertyName)]
        [String] $Name,

        [Parameter(Position = 1, ValueFromPipelineByPropertyName)]
        [Type] $Type,

        [Parameter(Position = 2, ValueFromPipelineByPropertyName)]
        [Nullable[int]] $Position,

        [Parameter(ValueFromPipelineByPropertyName)]
        [switch] $FromPipeline,
        
        [Parameter(ValueFromPipelineByPropertyName)]
        [switch] $FromPipelineProperty,

        [Parameter(ValueFromPipelineByPropertyName)]
        [switch] $Mandatory,

        [Parameter(ValueFromPipelineByPropertyName)]
        [String] $HelpMessage,

        [Parameter(ValueFromPipelineByPropertyName)]
        [String[]] $Set = $null,

        [Parameter(ValueFromPipelineByPropertyName)]
        [scriptblock] $GetData = $null,

        [Parameter(ValueFromPipelineByPropertyName)]
        [String[]] $Aliases = $null,

        [Parameter(ValueFromPipelineByPropertyName)]
        [System.Attribute[]] $Attributes 
    )

    if($null -eq $Type) {
        $Type = [String];
    }

    $attrs = New-Object System.Collections.ObjectModel.Collection[System.Attribute]

    $parameterAttr = New-Object System.Management.Automation.ParameterAttribute
    if($Position.HasValue) {
        $parameterAttr.Position = $Position.Value
    }
    $parameterAttr.ValueFromPipeline = $FromPipeline.ToBool()
    $parameterAttr.ValueFromPipelineByPropertyName = $FromPipelineProperty.ToBool()
    if(![string]::IsNullOrWhiteSpace($HelpMessage)) {
        $parameterAttr.HelpMessage = $HelpMessage
    }

    $parameterAttr.Mandatory = $Mandatory.ToBool()

    $attrs.Add($parameterAttr)

    if($Aliases -and $Aliases.Length) {
        $next = New-Object System.Management.Automation.AliasAttribute($Aliases)
        $attrs.Add($next)
      
    }

    if($GetData) {
        $Set = & $GetData
    }

    if($Set -and $Set.Length) {
        $next = New-Object System.Management.Automation.ValidateSetAttribute($Set);
        $attrs.Add($next);
    }

    if($attributes -and $attributes.Length)
    {
        foreach($attr in $attributes) {
            $attrs.Add($attr)
        }
    }

    $parameter = New-Object System.Management.Automation.RuntimeDefinedParameter($Name, $Type, $attrs)

    return $parameter
}


