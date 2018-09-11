function Update-PhpIni() {
    Param(
        [PsCustomObject] $Config 
    )

    $ini =  "/etc/php/7.2/fpm/php.ini" 
    $content = Get-Content $ini 
    $out = @();
    $found = @{}
    foreach($line in $content) {
        $l = $line;
        $Config | Get-Member -MemberType NoteProperty | ForEach-Object {
            $key = $_.Name
            $value = $config.$key 
            if($found[$key]) { return ; }
            if($value -is [bool]) {
                if($line -match ";$key =" -and $value -eq $true) {
                    $l = $line.Substring(1);
                    $found[$key]= $true;
                    return;
                }
                if($line.StartsWith("$key = ") -and $value -eq $false) {
                    $l = ";$line";
                    $found[$key]= $true;
                    return;
                }
            }
            if($line -match "$key =") {
                $l = "$key = $value";
                $found[$key] = $true;
                return;
            }
        }
        $out += $l;

        $out | Out-File $ini -Force
    }
}