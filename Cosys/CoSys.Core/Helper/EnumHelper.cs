using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoSys.Core
{
    public class EnumHelper
    {
        /// <summary>
        /// 根据枚举类型获取字典
        /// </summary>
        /// <param name="type">枚举类型</param>
        /// <returns></returns>
        public static Dictionary<int, string> GetDictionary(Type type)
        {
            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            type.GetFields().Where(x => x.FieldType.IsEnum).ToList().ForEach(x =>
            {
                var description = "";
                //取DescriptionAttribute特性的对象
                var descriptionAttribute = x.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault();
                if (descriptionAttribute == null)
                {
                    description = x.Name;
                }
                else
                {
                    description = (descriptionAttribute as DescriptionAttribute).Description;
                }
                dictionary.Add((int)x.GetValue(null), description);
            });
            return dictionary;
        }

        /// <summary>
        /// 取枚举描述值
        /// </summary>
        /// <param name="enumObj"></param>
        /// <returns></returns>
        public static string GetEnumDescription(object enumObj)
        {
            Type enumType = enumObj.GetType();
            var dictionary = GetDictionary(enumType);
            return dictionary[(int)enumObj];
        }


        /// <summary>
        /// 根据描述值获取枚举值
        /// </summary>
        /// <param name="enumObj"></param>
        /// <returns></returns>
        public static int GetEnumKey(Type enumObj, string description)
        {
            var dictionary = GetDictionary(enumObj);
            var key = dictionary.Where(x => x.Value.Equals(description,StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            return key.Key;
        }
    }
}
