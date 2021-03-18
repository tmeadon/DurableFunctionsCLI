function Get-ConnectedSubscriptionId
{
    [CmdletBinding()]
    param ()
    
    begin {}
    
    process
    {
        (Get-AzContext).Subscription.Id
    }
    
    end {}
}