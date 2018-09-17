$Env:GAINZ_INTERACTIVE = $null;

function Set-NonInteractive() {
    Param(
        [Switch] On,
        [Switch] Off
    )

    if($On.ToBool()) {
        $Env:GAINZ_INTERACTIVE = "0";
    }
    if($Off.ToBool()) {
        $Env:GAINZ_INTERACTIVE = "1";
    }
}