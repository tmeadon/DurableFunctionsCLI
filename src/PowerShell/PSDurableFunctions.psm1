# Dot source all public and private functions
Get-ChildItem -Path $PSScriptRoot -Filter "*.ps1" -Recurse | ForEach-Object {
    . $_.FullName
}

# Export public functions
Get-ChildItem -Path $PSScriptRoot\Public -Filter "*.ps1" -Recurse | ForEach-Object {
    Export-ModuleMember -Function $_.BaseName
}