function Build-TaskHubFinder
{
    [CmdletBinding()]
    param
    (
        # Subscription ID
        [Parameter(Mandatory)]
        [string]
        $SubscriptionId,

        # Azure bearer token
        [Parameter(Mandatory)]
        [string]
        $Token,

        # Resource group name
        [Parameter(Mandatory)]
        [string]
        $ResourceGroupName,

        # Storage account name
        [Parameter()]
        [string]
        $StorageAccountName
    )
    
    begin {}
    
    process
    {
        $factory = [DurableFunctionsCLI.Core.Discovery.TaskHubFinderFactory]::new($SubscriptionId, $Token).InResourceGroup($ResourceGroupName)
        if ($StorageAccountName) { $factory = $factory.InStorageAccount($StorageAccountName) }
        $factory.Build()
    }
    
    end {}
}
