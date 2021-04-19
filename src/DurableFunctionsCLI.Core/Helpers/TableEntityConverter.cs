using DurableFunctionsCLI.Core.Models;
using System;
using System.Linq;
using System.Reflection;

namespace DurableFunctionsCLI.Core.Helpers
{
    public static class TableEntityConverter
    {
        public static T ConvertToBaseType<T>(T tableEntity) where T : new()
        {
            var output = new T();

            foreach (var prop in output.GetType().GetProperties().ToList())
            {
                var value = tableEntity.GetType().GetProperty(prop.Name).GetValue(tableEntity, null);
                prop.SetValue(output, value, null);
            }

            return output;
        }
    }
}