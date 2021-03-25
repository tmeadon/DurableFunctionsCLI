using System;

namespace DurableFunctionsCLI.Core.Exceptions
{
    public class StorageAccountNotFoundException : Exception
    {
        public StorageAccountNotFoundException() : base() {}
    }
}