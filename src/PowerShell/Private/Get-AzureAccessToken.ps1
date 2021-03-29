function Get-AzureAccessToken
{
    [CmdletBinding()]
    param ()
    
    begin {}
    
    process
    {
        (Get-AzAccessToken -ErrorAction Stop).Token
    }
    
    end {}
}