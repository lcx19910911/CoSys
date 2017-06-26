using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using CoSys.Model;
using CoSys.Core;
using CoSys.Repository;
using System.Linq;

namespace CoSys.Test
{
    [TestClass]
    public class UnitTest1
    {
        string connectStr = @"data source=PC-20170504PCLA\SQLEXPRESS;user id=sa;password=123456;database=convert";
        string noids = "15864946949,13960630573,13774870485,18060314851";
        [TestMethod]
        public void TestMethod1()
        {
            var newList = new List<OldUser>();
            using (SqlConnection con = new SqlConnection(connectStr))
            {
                con.Open();
                var dicPhone = new List<string>();
                var list = GetListResult("select register_name,register_phone,register_time,register_membertype,province,city,canton,town from js_shuju", con);
                if (list != null && list.Count > 0)
                {
                    list.ForEach(x =>
                    {
                        if (!dicPhone.Contains(x.register_phone))
                        {
                            newList.Add(x);
                            dicPhone.Add(x.register_phone);
                        }
                    });
                }
            }

            var ss = "1";
            StringBuilder sb = new StringBuilder();
            string noExitsList = "";
            if (newList.Count > 0)
            {
                using (var db = new DbRepository())
                {
                    var dic = db.DataDictionary.Where(x => x.GroupCode == GroupCode.Area).ToDictionary(x => x.Key);
                    var list = dic.Values.ToList();
                    var key = list.Max(y => y.Key.GetInt() + 1);
                    newList.ForEach(x =>
                    {
                        if (x.canton.IsNotNullOrEmpty() && dic.ContainsKey(x.canton))
                        {
                            x.register_name=x.register_name.Replace("'", "");
                            x.register_phone=x.register_phone.Replace("'", "");
                            var keyValue = dic[x.canton];
                            var searchList = list.Where(y => !string.IsNullOrEmpty(y.ParentKey) && y.ParentKey == x.canton).ToList();
                            var one = searchList.FirstOrDefault(y => y.Value == x.town);
                            if (one == null)
                            {
                                var newMode = new DataDictionary()
                                {
                                    ID = Guid.NewGuid().ToString("N"),
                                    ParentKey = x.canton,
                                    GroupCode = GroupCode.Area,
                                    Key = key.ToString(),
                                    Value = x.town
                                };
                                db.DataDictionary.Add(newMode);
                                dic.Add(key.ToString(), newMode);
                                one = newMode;
                                key++;
                            }
                            sb.Append("insert into dbo.[User] (ID,Account,Password,RealName,PenName,ProvoniceCode,CityCode,CountyCode,StreetCode,Phone,Code,CreatedTime,IsDelete) ");
                            sb.Append($"values('{Guid.NewGuid().ToString("N")}','{x.register_phone}','96e79218965eb72c92a549dd5a330112','{x.register_name}','{x.register_name}','{x.province}','{x.city}','{x.canton}','{one.Key}','{x.register_phone}',4,'{GetTime(x.register_time)}',0);\r\n");
                        }
                        else
                        {
                            noExitsList += "phone=" + x.register_phone + ",\r\n";
                        }
                    });
                }
            }

            var sqlStr = sb.ToString();

        }
        /// 时间戳转为C#格式时间  
        /// </summary>  
        /// <param name=”timeStamp”></param>  
        /// <returns></returns>  
        private DateTime GetTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime); return dtStart.Add(toNow);
        }
        public List<OldUser> GetListResult(string sqlcomand, SqlConnection con)
        {
            SqlCommand cmd = new SqlCommand(sqlcomand, con);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);

            DataSet ds = new DataSet();

            adapter.Fill(ds);//填充数据

            if (ds.Tables[0] != null)
                return ModelConvertHelper<OldUser>.ToModels(ds.Tables[0]) as List<OldUser>;
            else
                return null;
        }

    }



    /// <summary>
    /// 数据库表转化为实体帮助类，使用此类注意数据库表的字段和实体属性名称一样，并且实体存在显式定义的无参构造函数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class ModelConvertHelper<T> where T : new()
    {
        /// <summary>
        /// 将DataTable转为Model
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <returns>泛型实体集合</returns>
        public static IList<T> ToModels(DataTable dt)
        {
            IList<T> ts = new List<T>();
            foreach (DataRow dr in dt.Rows)
            {
                ts.Add(ToModel(dr));
            }
            return ts;
        }

        /// <summary>
        /// 将SqlDataReader读取的内容转为Model，结束后不会自动关闭Reader
        /// </summary>
        /// <param name="dr">SqlDataReader</param>
        /// <returns>泛型实体集合</returns>
        public static IList<T> ToModels(SqlDataReader dr)
        {
            IList<T> ts = new List<T>();
            while (dr.Read())
            {
                ts.Add(ToModel(dr));
            }
            return ts;
        }

        /// <summary>
        /// 将DataRow读取到的一行 转为 Model
        /// </summary>
        /// <param name="dr">DataRow</param>
        /// <returns>泛型实体</returns>
        public static T ToModel(DataRow dr)
        {
            // 获得此模型的类型
            Type type = typeof(T);
            string tempName = "";
            T t = new T();
            // 获得此模型的公共属性
            PropertyInfo[] propertys = t.GetType().GetProperties();
            DataTable dt = dr.Table;
            foreach (PropertyInfo pi in propertys)
            {
                tempName = pi.Name;
                if (dt.Columns.Contains(tempName))
                {
                    // 判断此属性是否有Setter6
                    if (!pi.CanWrite)
                        continue;
                    object value = dr[tempName];
                    if (value != DBNull.Value)
                    {
                        if (pi.PropertyType.IsEnum)
                            pi.SetValue(t, Enum.Parse(pi.PropertyType, value.ToString().Trim(), true), null);
                        else if (pi.PropertyType == typeof(DateTime) || pi.PropertyType == typeof(DateTime?))
                            pi.SetValue(t, Convert.ToDateTime(value.ToString()), null);
                        else
                            pi.SetValue(t, value, null);
                    }
                }
            }
            return t;
        }

        /// <summary>
        /// 将 SqlDataReader 转为Model, 如果 SqlDataReader.read() 有值 ，返回对象，否则返回Null
        /// </summary>
        /// <param name="dr">SqlDataReader</param>
        /// <returns>泛型实体</returns>
        public static T ToModel(SqlDataReader dr)
        {
            // 获得此模型的类型
            Type type = typeof(T);
            string tempName = "";
            T t = new T();
            // 获得此模型的公共属性
            PropertyInfo[] propertys = t.GetType().GetProperties();
            int clen = dr.FieldCount;
            Dictionary<string, object> nv = new Dictionary<string, object>();
            for (int i = 0; i < clen; i++)
            {
                string fieldname = dr.GetName(i).ToLower();
                nv[fieldname] = dr[i];
            }
            foreach (PropertyInfo pi in propertys)
            {
                tempName = pi.Name.ToLower();
                if (nv.ContainsKey(tempName))
                {
                    if (!pi.CanWrite)
                        continue;
                    object value = nv[tempName];
                    if (value != DBNull.Value)
                    {
                        if (pi.PropertyType.IsEnum)
                            pi.SetValue(t, Enum.Parse(pi.PropertyType, value.ToString().Trim(), true), null);
                        else if (pi.PropertyType == typeof(DateTime) || pi.PropertyType == typeof(DateTime?))
                            pi.SetValue(t, Convert.ToDateTime(value.ToString()), null);
                        else
                            pi.SetValue(t, value, null);
                    }
                }
            }
            return t;
        }

    }

    public class OldUser
    {
       public string register_name { get; set; }
       public string  register_phone{ get; set; }
       public string  register_time{ get; set; }
       public string  register_membertype{ get; set; }
       public string  province{ get; set; }
       public string  city{ get; set; }
       public string  canton{ get; set; }
       public string  town{ get; set; }
    }
}
