function Resolve-KwehStringTemplate() {
    Param(
        [Parameter(Position = 0)]
        [String] $Template,

        [Parameter(Position = 1, ValueFromPipeline = $True)]
        [PsCustomObject] $Model 
    )

    $eval = {  
        param($m) 
        
        $var = $m.Value.TrimStart('{').TrimEnd('}').Trim()
        if($var -match "env:") {
            $parts = $var.Split(':')
            $name = $parts[1];
            return (Get-Item "Env:/$Name").Value 
        }
        if($var.Contains(".")) {
            $parts = $var.Split(".")
            $m = $Model;
            for($i = 0; $i -lt $parts.Length; $i++) {
                $next = $parts[$i]
                $m = $m.$next 
                if($m -eq $null) {
                    return $null;
                }
            }
            return $m;
        }
        $m = $Model.$var 
        return $m 
    }

    $pattern = [Regex]"{{\s*[\w\.:]+\s*}}"
   
    $result = $pattern.Replace($Template, $eval)
 
    return $result
}
Set-Alias -Name Resolve-StringTemplate -Value Resolve-KwehStringTemplate

