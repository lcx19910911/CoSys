using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoSys.Core
{
    public class SearchModifyHelper
    {

        #region 复制属性（反射实现）

        /// <summary>
        /// 将对象属性赋值到实体中，只复制基本类型（反射实现）
        /// </summary>
        public static string CompareProperty<T, V>(T entity, V value)
        {
            var targetProps = typeof(T).GetProperties();
            var valueProps = typeof(V).GetProperties();
            StringBuilder msg = new StringBuilder();
            if(!typeof(T).Equals(typeof(V)))
            {
                return string.Empty;
            }
            foreach (PropertyInfo targetPi in targetProps)
            {

                try
                {  
                    var valuePi = valueProps.FirstOrDefault(p => p.Name == targetPi.Name);
                    string name = string.Empty;
                    var descriptionAttribute = targetPi.GetCustomAttributes(typeof(DisplayAttribute)).FirstOrDefault();
                    if (descriptionAttribute != null)
                    {
                        name = (descriptionAttribute as DisplayAttribute).Name;
                    }
                    else
                    {
                        name = targetPi.Name;
                    }
                    if (!valuePi.GetValue(entity).ToString().Equals(targetPi.GetValue(value).ToString()))
                    {
                        msg.AppendFormat("{0}原值{1}，修改后{2}\r\n", name,valuePi.GetValue(entity), targetPi.GetValue(value));
                    }
                }
                catch
                {
                }
            }

            return msg.ToString();
        }

        #endregion

    }
}
