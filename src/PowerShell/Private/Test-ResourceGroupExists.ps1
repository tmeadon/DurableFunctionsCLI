function Test-ResourceGroupExists
{
    [CmdletBinding()]
    param
    (
        # name of the resource group to test for existence
        [Parameter(Mandatory)]
        [string]
        $ResourceGroupName
    )
    
    begin {}
    
    process
    {
        $rg = Get-AzResourceGroup -Name $ResourceGroupName -ErrorAction SilentlyContinue

        if ($rg) { return $true }

        return $false
    }
    
    end {}
}