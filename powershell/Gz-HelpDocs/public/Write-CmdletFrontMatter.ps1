

function Write-CmdletFrontMatter() {
    Param(
        [Parameter(Position = 0)]
        [PsCustomObject] $Model
    )
    $name = $Model.Name;

    if($name) {
        $hyphenatedName = $model.HyphenatedName
        $now = [DateTime]::UtcNow
        $now = $now.ToString()

        $out = "---`n"
        $out += "title: $name`n"
        if($model.Author) {
            $out += "authors: [`"$($model.Author)`"]`n"
        }
        if($model.PublishedAt) {
            $out += "publishedAt: $($model.PublishedAt)`n"
        }
        if($model.Company) {
            $out += "company: $($model.PublishedAt)`n"
        }
        if($model.Version) {
            $out += "version: $($model.Version)`n"
        }
        if($model.ModuleName) {
            $out += "module: $($model.ModuleName)`n"
        }
        if($model.UpdatedAt) {
            $out += "updatedAt: $($model.UpdatedAt)`n"
        }
        $uri = $hyphenatedName + ".md"
        if($model.BaseUri) {
            $uri = $model.BaseUri + "/" + $hyphenatedName + ".md"
        }
        if($model.Tags -and $model.Tags.Count) {
            $tags = [String]::Join("`",`"", $model.Tags)
            $out += "tags: [`"$tags`"]`n"
        }
        $out += "uri: $uri`n"

        $out += "---`n`n"

        return $out;
    }

    return ""
}