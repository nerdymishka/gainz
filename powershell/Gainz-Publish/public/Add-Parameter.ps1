


function Add-StringParameter() {
    Param(
        [Parameter(ValueFromPipeline = $true)]
        [Hashtable] $Set,

        [Parameter(Position  = 0)]
        [String] $Name,

        [Parameter(Position = 1)]
        [String] $Value
    )

    if(![string]::IsNullOrWhiteSpace($Value)) {
        $Set.Add($Name, $Value)
    }
}

function Add-SwitchParameter() {
    Param(
        [Parameter(ValueFromPipeline = $true)]
        [Hashtable] $Set,

        [Parameter(Position  = 0)]
        [String] $Name,

        [Parameter(Position = 1)]
        [boolean] $Value
    )

    if($Value) {
        $Set.Add($Name, $Value)
    }
}

function Add-ObjectParameter() {
    Param(
        [Parameter(ValueFromPipeline = $true)]
        [Hashtable] $Set,

        [Parameter(Position  = 0)]
        [String] $Name,

        [Parameter(Position = 1)]
        [Object] $Value
    )

    if($Value) {
        $Set.Add($Name, $Value)
    }
}