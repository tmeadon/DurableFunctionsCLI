using System;

namespace DurableFunctionsCLI.Core.Exceptions
{
    public class StorageApiThrottledException : Exception
    {
        public StorageApiThrottledException() : base() {}
    }
}