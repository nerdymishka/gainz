

function Read-InteractiveChoice() {
    Param(
        [String] $Message = "Are you sure?",
        [String[]] $Choices = $null,
        [String] $Title = $null,
        [Int32] $DefaultValue = 0
    )

    
    if($null -eq $Choices -or $Choices.Length -eq 0) {
        $Choices = @("&No", "&Yes");
    }

    if($DefaultValue -ge $Choices.Length) {
        throw "DefaultValue must not exceed the number of choices";
    }

    if(Test-NonInteractive) {
        return $DefaultValue
    }

    return $host.UI.PromptForChoice($null, $Message, $Choices, $DefaultValue)
}

