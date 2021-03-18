function Get-AzureAccessToken
{
    [CmdletBinding()]
    param ()
    
    begin {}
    
    process
    {
        (Get-AzAccessToken).Token
    }
    
    end {}
}