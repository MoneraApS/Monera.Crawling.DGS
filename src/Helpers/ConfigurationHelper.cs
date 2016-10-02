using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monera.Crawling.DGS.Helpers
{
    public static class ConfigurationHelper
    {
        public static T GetValue<T>(string key, T defaultValue)
        {
            T result = defaultValue;
            var stringValue = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrEmpty(stringValue))
                return result;

            Type typeT = typeof(T);
            if (typeT.IsPrimitive || typeT == typeof(string))
            {
                return (T)Convert.ChangeType(stringValue, typeT);
            }
            else if (typeT.IsEnum)
            {
                return (T)System.Enum.Parse(typeT, stringValue); // Yeah, we're making an assumption
            }
            else
            {
                var convertible = result as IConvertible;
                if (convertible != null)
                {
                    return (T)convertible.ToType(typeT, CultureInfo.InvariantCulture);
                }
            }
            return result;
        }
        public static T GetValue<T>(string key)
        {
            return GetValue(key, default(T));
        }
    }
}
