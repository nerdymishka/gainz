
$is64Bit = $null;

function Test-64Bit() {
    Param(
        [Switch] $Force 
    )

    if($null -eq $is64Bit -or $Force.ToBool()) {
        $is64Bit = GEt-IntPtrSize -eq 8;
    }

    return $is64Bit;
}