function Add-JsonDeserialisedMember
{
    [CmdletBinding()]
    param
    (
        # Object to add the deserialised member to
        [Parameter(Mandatory)]
        [object]
        $InputObject,

        # Name of the object member to deserialise
        [Parameter(Mandatory)]
        [string]
        $MemberName
    )
    
    begin {}
    
    process
    {
        if (($InputObject | Get-Member).Name -notcontains $MemberName) { throw "Member $MemberName not found on object $InputObject" }

        $newMemberName = $MemberName + "Object"

        if (($null -ne $InputObject.$MemberName) -and (Test-Json -Json $InputObject.$MemberName -ErrorAction SilentlyContinue))
        {
            $newMemberValue = $InputObject.$MemberName | ConvertFrom-Json
        }
        else
        {
            $newMemberValue = $InputObject.$MemberName
        }

        $InputObject | Add-Member -Name $newMemberName -MemberType NoteProperty -Value $newMemberValue
    }
    
    end {}
}