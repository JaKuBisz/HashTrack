using System;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;
using HashTrack.Core.Attributes;

namespace HashTrack.UI.Helpers
{
    public class EnumFriendlyNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Enum enumValue)
            {
                return GetDisplayName(enumValue);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string displayName)
            {
                foreach (var field in targetType.GetFields(BindingFlags.Public | BindingFlags.Static))
                {
                    if (Attribute.GetCustomAttribute(field, typeof(DisplayNameAttribute)) is DisplayNameAttribute attribute && attribute.DisplayName == displayName)
                    {
                        return (Enum)field.GetValue(null);
                    }
                }
            }
            throw new ArgumentException("Invalid display name", nameof(value));        }

        public static string GetDisplayName(Enum value)
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            if (name != null)
            {
                var field = type.GetField(name);
                if (field != null)
                {
                    if (Attribute.GetCustomAttribute(field, typeof(DisplayNameAttribute)) is DisplayNameAttribute attr)
                    {
                        return attr.DisplayName;
                    }
                }
            }
            return null;  // or return name if no DisplayName attribute is found
        }
    }
}