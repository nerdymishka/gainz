

function Read-InteractiveChoice() {
    Param(
        [String] $Message = "Are you sure?",
        [String[]] $Choices = $null,
        [String] $Title = $null,
        [Int32] $DefaultValue = 1
    )

    
    if($null -eq $Choices -or $Choices.Length -eq 0) {
        $Choices = @("&Yes", "&No");
    }

    if($DefaultValue -ge $Choices.Length) {
        throw "DefaultValue must not exceed the number of choices";
    }

    if(Test-ShellNonInteractive) {
        return $Choices[$DefaultValue].Replace("&", "");
    }

    return $host.UI.PromptForChoice($null, $Message, $Choices, $DefaultValue)
}