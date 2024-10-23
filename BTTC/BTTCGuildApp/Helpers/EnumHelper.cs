using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Logging;

namespace BTTCGuildApp.Helpers
{
    public static class EnumHelper
    {
        public static TEnum? GetEnumFromStringValue<TEnum>(string stringValue) where TEnum : Enum
        {
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                return default;
            }
            // Check if the enum type TEnum has any fields
            foreach (var field in typeof(TEnum).GetFields())
            {
                // Get the StringValueAttribute associated with the enum field
                StringValueAttribute[] attribute = field.GetCustomAttributes(
                    typeof(StringValueAttribute), false) as StringValueAttribute[];
                if (attribute != null && attribute.Length > 0 && attribute[0].StringValue.Contains(stringValue, StringComparison.OrdinalIgnoreCase))
                {
                    return (TEnum)field.GetValue(null);
                }
            }
            return default;
        }
    }
}