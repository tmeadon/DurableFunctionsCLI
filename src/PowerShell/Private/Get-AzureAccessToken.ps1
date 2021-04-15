function Get-AzureAccessToken
{
    [CmdletBinding()]
    param ()
    
    begin {}
    
    process
    {
        try 
        {
            (Get-AzAccessToken -ErrorAction Stop).Token
        }
        catch
        {
            throw "Unable to get Azure access token, please ensure you have signed in by running Connect-AzAccount"
        }
    }
    
    end {}
}