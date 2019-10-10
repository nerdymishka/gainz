function Add-DbParameter() {
    Param(
        [Parameter(Mandatory = $true, Position = 1, ValueFromPipeline = $true)]
        [System.Data.IDbCommand] $Command,
        
        [Parameter(Position = 2)]
        [Object] $Parameters,
        
        [Parameter(Position = 3)]
        [String] $ParameterPrefix,
        
        [switch] $Update
    )

   
    
     if($Parameters) {
            if([string]::IsNullOrWhiteSpace($ParameterPrefix)) {
                $ParameterPrefix = Get-DbParameterPrefix
            }

            if($Parameters -is [System.Management.Automation.PSCustomObject]) {
                if($Update.ToBool()) {
                     $Parameters | Get-Member -MemberType NoteProperty | Foreach-Object {
                        $Name = $_.Name
                        $Name = "$($ParameterPrefix)$Name" 
                        $Value = $Parameters.$Name 
                        $Command.Parameters[$Name].Value = $Value
                    }
                } else {
                    $Parameters | Get-Member -MemberType NoteProperty | Foreach-Object {
                        $Name = $_.Name
                        $Value = $Parameters.$Name 
                        $p = $Command.CreateParameter();
                        $p.ParameterName = "$($ParameterPrefix)$Name" 
                        $p.Value = $Value 
                        $Command.Parameters.Add($p) | Out-Null
                    }
                }
              
            } elseif($Parameters -is [System.Collections.IDictionary]) {
               
                if($Update.ToBool()) {
                    foreach($key in $Parameters.Keys) {
                        $Value = $Parameters[$key]
                        $Name = "$($ParameterPrefix)$key"
                        $Command.Parameters[$Name].Value = $Value;
                        
                    }
                } else {
                     foreach($key in $Parameters.Keys) {
                        $Value = $Parameters[$key]
                        $Name = "$($ParameterPrefix)$key"
                        $p = $command.CreateParameter()
                        $p.ParameterName = $Name 
                        $p.Value = $Value
                        $Command.Parameters.Add($p) | Out-Null
                    }
                }
               
            } elseif($Parameters -is [System.Collections.IList]) {
                if($Update.ToBool()) {
                     for($i = 0; $i -lt $Parameters.Length; $i++) {
                        $Name = "$($ParameterPrefix)$i"
                        $Value = $Parameters[$i]
                        $p = $Command.Parameters[$Name].Value = $Value;
                    }
                } else {

                    for($i = 0; $i -lt $Parameters.Length; $i++) {
                        $value = $Parameters[$i];
                        if($value -is [System.Data.IDbParameter]) {
                            $Command.Parameters.Add($value) | Out-Null
                            continue;
                        }
                        $p = $Command.Parameter();
                        $p.ParameterName = "$($ParameterPrefix)$i"
                        $p.Value = $Value 
                        $Command.Parameters.Add($p) | Out-Null
                    }
                }
              
            } else {
                throw Exception "Parameters does not support type [$($Parameters.GetType())]"
            }
        }
}