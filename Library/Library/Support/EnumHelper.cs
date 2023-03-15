using System.ComponentModel;

namespace Library.Support
{
    public static class EnumHelper
    {
        public static string GetDescription<T>(this T enumValue) where T : Enum, IConvertible
        {
            var description = enumValue.ToString();
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

            if (fieldInfo != null)
            {
                var attrs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (attrs != null && attrs.Length > 0)
                {
                    description = ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            return description;
        }

        public static T GetEnumValueByDescription<T>(this string description) where T : Enum
        {
            foreach (Enum enumItem in Enum.GetValues(typeof(T)))
            {
                if (enumItem.GetDescription() == description)
                {
                    return (T)enumItem;
                }
            }
            throw new ArgumentException(nameof(description));
        }
    }
}
