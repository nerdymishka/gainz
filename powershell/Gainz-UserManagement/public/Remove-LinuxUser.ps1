Function Remove-LinuxUser() {
    Param(
        [Parameter(Position = 0)]
        [String] $Name,

        [Alias("r")]
        [Switch] $Remove,

        [Alias("f")]
        [switch] $Force 
    )

    if([string]::IsNullOrWhiteSpace($Name)) {
        $noninteractive = Test-Noninteractive;
        if($noninteractive) {
            throw [System.ArgumentNullException] "-Name"
            return;
        }

        While([string]::IsNullOrWhiteSpace($Name)) {
            $Name = Read-Host -Prompt "Name"
        }
    }

    $splat = @();

    if($Remove.ToBool()) {
        $splat += "-r"
    }

    if($Force.ToBool()) {
        $splat += "-f"
    }

    userdel @splat $Name 
}