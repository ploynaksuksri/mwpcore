using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Mwp.Extensions
{
    public static class EnumExtensions
    {
        public static T GetAttribute<T>(this Enum enumVal)
            where T : Attribute
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);

            return attributes.Length > 0 ? (T)attributes[0] : null;
        }

        public static string GetName<T>(this T enumValue)
            where T : Enum
        {
            var displayAttribute = enumValue.GetAttribute<DisplayAttribute>();

            return displayAttribute?.GetName();
        }

        public static string GetShortName<T>(this T enumValue)
            where T : Enum
        {
            var displayAttribute = enumValue.GetAttribute<DisplayAttribute>();

            return displayAttribute?.GetShortName();
        }

        public static string GetDescription<T>(this T enumValue)
            where T : Enum
        {
            var displayAttribute = enumValue.GetAttribute<DisplayAttribute>();

            if (displayAttribute != null)
            {
                return displayAttribute.GetDescription();
            }

            var descriptionAttribute = enumValue.GetAttribute<DescriptionAttribute>();

            return descriptionAttribute?.Description;
        }

        public static string GetGroupName<T>(this T enumValue)
            where T : Enum
        {
            var displayAttribute = enumValue.GetAttribute<DisplayAttribute>();

            return displayAttribute?.GetGroupName();
        }
    }
}