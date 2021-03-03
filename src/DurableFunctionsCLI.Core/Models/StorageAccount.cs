using System.Collections.Generic;

namespace DurableFunctionsCLI.Core.Models
{
    internal class StorageAccount
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string Kind { get; set; }
        public StorageAccountProperties Properties { get; set; }
    }

    internal class StorageAccountProperties
    {
        public Dictionary<string, string> PrimaryEndpoints { get; set; }
    }
}