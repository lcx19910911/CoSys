﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Security.Cryptography;
using CoSys.Core;

namespace CoSys.Core
{
    /// <summary>
    /// 字符串扩展方法
    /// </summary>
    public static class StringExtensions
    {
        #region 字符串操作
        /// <summary>
        /// 获取字符串的实际长度(按单字节)
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static int GetRealLength(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return 0;
            return Encoding.Default.GetByteCount(source);
        }

        /// <summary>
        /// 取得固定长度的字符串(按单字节截取)。
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="resultLength">截取长度</param>
        /// <returns></returns>
        public static string SubString(this string source, int resultLength)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;

            //判断字符串长度是否大于截断长度
            if (Encoding.Default.GetByteCount(source) > resultLength)
            {
                //初始化
                int i = 0, j = 0;

                //为汉字或全脚符号长度加2否则加1
                foreach (char newChar in source)
                {
                    if (newChar > 127)
                    {
                        i += 2;
                    }
                    else
                    {
                        i++;
                    }
                    if (i > resultLength)
                    {
                        source = source.Substring(0, j);
                        break;
                    }
                    j++;
                }
            }
            return source;
        }

        /// <summary>
        /// 取得固定长度字符的字符串，后面加上…(按单字节截取)
        /// </summary>
        /// <param name="source"></param>
        /// <param name="resultLength"></param>
        /// <returns></returns>
        public static string SubStr(this string source, int resultLength)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;

            if (source.GetRealLength() <= resultLength)
            {
                return source;
            }
            else
            {
                return source.SubString(resultLength) + "...";
            }
        }
        #endregion

        #region 字符串格式验证
        /// <summary>
        /// 判断字符串是否为null或为空.判断为空操作前先进行了Trim操作。
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string source)
        {
            if (source != null)
            {
                return source.Trim().Length < 1;
            }
            return true;
        }
        /// <summary>
        /// 判断字符串是否为整型
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsInteger(this string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return false;
            }
            int i;
            return Int32.TryParse(source, out i);
        }

        /// <summary>
        /// Email 格式是否合法
        /// </summary>
        /// <param name="source"></param>
        public static bool IsEmail(this string source)
        {
            return Regex.IsMatch(source, @"^\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z0-9]+$");
        }

        /// <summary>
        /// 判断是否公网IP
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsPublicIP(this string source)
        {
            return Regex.IsMatch(source, @"^(((25[0-5]|2[0-4][0-9]|19[0-1]|19[3-9]|18[0-9]|17[0-1]|17[3-9]|1[0-6][0-9]|1[1-9]|[2-9][0-9]|[0-9])\.(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9]))|(192\.(25[0-5]|2[0-4][0-9]|16[0-7]|169|1[0-5][0-9]|1[7-9][0-9]|[1-9][0-9]|[0-9]))|(172\.(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|1[0-5]|3[2-9]|[4-9][0-9]|[0-9])))\.(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\.(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])$");
        }

        /// <summary>
        /// 验证IP
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsIP(this string source)
        {
            return Regex.IsMatch(source, @"^(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])$");
        }

        /// <summary>
        /// 检查字符串是否为A-Z、0-9及下划线以内的字符
        /// </summary>
        /// <param name="source">被检查的字符串</param>
        /// <returns>是否有特殊字符</returns>
        public static bool IsLetterOrNumber(this string source)
        {
            bool b = System.Text.RegularExpressions.Regex.IsMatch(source, @"\w");
            return b;
        }

        /// <summary>
        /// 验输入字符串是否含有“/\:.?*|$]”特殊字符
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsSpecialChar(this string source)
        {
            Regex r = new Regex(@"[/\<>:.?*|$]");
            return r.IsMatch(source);
        }

        /// <summary>
        /// 是否全为中文/日文/韩文字符
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns></returns>
        public static bool IsChineseChar(this string source)
        {
            //中文/日文/韩文: [\u4E00-\u9FA5]
            //英文:[a-zA-Z]
            return Regex.IsMatch(source, @"^[\u4E00-\u9FA5]+$");
        }

        /// <summary>
        /// 是否包含双字节字符(允许有单字节字符)
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsDoubleChar(this string source)
        {
            return Regex.IsMatch(source, @"[^\x00-\xff]");
        }

        /// <summary>
        /// 是否为日期型字符串
        /// </summary>
        /// <param name="source">日期字符串(2005-6-30)</param>
        /// <returns></returns>
        public static bool IsDate(this string source)
        {
            return Regex.IsMatch(source, @"^((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-8]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))$");
        }


        /// <summary>
        /// 是否为时间型字符串
        /// </summary>
        /// <param name="source">时间字符串(15:00:00)</param>
        /// <returns></returns>
        public static bool IsTime(this string source)
        {
            return Regex.IsMatch(source, @"^((20|21|22|23|[0-1]?\d):[0-5]?\d:[0-5]?\d)$");
        }

        /// <summary>
        /// 是否为日期+时间型字符串
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsDateTime(this string source)
        {
            return Regex.IsMatch(source, @"^(((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-8]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-)) (20|21|22|23|[0-1]?\d):[0-5]?\d:[0-5]?\d)$");
        }

        /// <summary>
        /// 是否为文件物理路径
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsPhysicalPath(this string source)
        {
            return Regex.IsMatch(source, @"^[a-zA-Z]:[\\/]+(?:[^\<\>\/\\\|\:""\*\?\r\n]+[\\/]+)*[^\<\>\/\\\|\:""\*\?\r\n]*$");
        }

        #endregion

        #region 字符串编码
        /// <summary>
        /// 将字符串使用base64算法加密
        /// </summary>
        /// <param name="source">待加密的字符串</param>
        /// <returns>加码后的文本字符串</returns>
        public static string ToBase64(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            return Convert.ToBase64String(Encoding.Default.GetBytes(source));
        }
        /// <summary>
        /// 从Base64编码的字符串中还原字符串，支持中文
        /// </summary>
        /// <param name="source">Base64加密后的字符串</param>
        /// <returns>还原后的文本字符串</returns>
        public static string FromBase64(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            return Encoding.Default.GetString(Convert.FromBase64String(source));
        }

        /// <summary>
        /// 将 GB2312 值转换为 UTF8 字符串(如：测试 -> 娴嬭瘯 )
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string FromGBToUTF8(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            return Encoding.GetEncoding("GB2312").GetString(Encoding.UTF8.GetBytes(source));
        }

        /// <summary>
        /// 将 UTF8 值转换为 GB2312 字符串 (如：娴嬭瘯 -> 测试)
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string FromUTF8ToGB(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            return Encoding.UTF8.GetString(Encoding.GetEncoding("GB2312").GetBytes(source));
        }


        /// <summary>
        /// 由16进制转为汉字字符串（如：B2E2 -> 测 ）
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string FromHex(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            byte[] oribyte = new byte[source.Length / 2];
            for (int i = 0; i < source.Length; i += 2)
            {
                string str = Convert.ToInt32(source.Substring(i, 2), 16).ToString();
                oribyte[i / 2] = Convert.ToByte(source.Substring(i, 2), 16);
            }
            return Encoding.Default.GetString(oribyte);
        }

        /// <summary>
        /// 字符串转为16进制字符串（如：测 -> B2E2 ）
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToHex(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            int i = source.Length;
            string temp;
            string end = "";
            byte[] array = new byte[2];
            int i1, i2;
            for (int j = 0; j < i; j++)
            {
                temp = source.Substring(j, 1);
                array = Encoding.Default.GetBytes(temp);
                if (array.Length.ToString() == "1")
                {
                    i1 = Convert.ToInt32(array[0]);
                    end += Convert.ToString(i1, 16);
                }
                else
                {
                    i1 = Convert.ToInt32(array[0]);
                    i2 = Convert.ToInt32(array[1]);
                    end += Convert.ToString(i1, 16);
                    end += Convert.ToString(i2, 16);
                }
            }
            return end.ToUpper();
        }

        /// <summary>
        /// 字符串转为unicode字符串（如：测试 -> &#27979;&#35797;）
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToUnicode(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            StringBuilder sa = new StringBuilder();//Unicode
            string s1;
            string s2;
            for (int i = 0; i < source.Length; i++)
            {
                byte[] bt = Encoding.Unicode.GetBytes(source.Substring(i, 1));
                if (bt.Length > 1)//判断是否汉字
                {
                    s1 = Convert.ToString((short)(bt[1] - '\0'), 16);//转化为16进制字符串
                    s2 = Convert.ToString((short)(bt[0] - '\0'), 16);//转化为16进制字符串
                    s1 = (s1.Length == 1 ? "0" : "") + s1;//不足位补0
                    s2 = (s2.Length == 1 ? "0" : "") + s2;//不足位补0
                    sa.Append("&#" + Convert.ToInt32(s1 + s2, 16) + ";");
                }
            }

            return sa.ToString();
        }


        /// <summary>
        /// 字符串转为UTF8字符串（如：测试 -> \u6d4b\u8bd5）
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToUTF8(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            StringBuilder sb = new StringBuilder();//UTF8
            string s1;
            string s2;
            for (int i = 0; i < source.Length; i++)
            {
                byte[] bt = Encoding.Unicode.GetBytes(source.Substring(i, 1));
                if (bt.Length > 1)//判断是否汉字
                {
                    s1 = Convert.ToString((short)(bt[1] - '\0'), 16);//转化为16进制字符串
                    s2 = Convert.ToString((short)(bt[0] - '\0'), 16);//转化为16进制字符串
                    s1 = (s1.Length == 1 ? "0" : "") + s1;//不足位补0
                    s2 = (s2.Length == 1 ? "0" : "") + s2;//不足位补0
                    sb.Append("\\u" + s1 + s2);
                }
            }

            return sb.ToString();
        }
        /// <summary>
        /// 将字符串转为安全的Sql字符串，不建议使用。尽可能使用参数化查询来避免
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToSafeSql(this string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.Empty;
            }
            else
            {
                return source.Replace("'", "''");
            }
        }
        /// <summary>
        /// 将字符串转换化安全的js字符串值（对字符串中的' "进行转义) 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToSafeJsString(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            source = source.Replace("'", "\\'");
            source = source.Replace("\"", "\\\"");
            source = source.Replace("\r\n", "").Replace("\r", "").Replace("\n", "");
            return source;
        }

        /// <summary>
        /// 注释like操作字符串中出现的特殊符号
        /// </summary>
        /// <remarks>注意：如果like查询中本身有使用到特殊字符，请不要使用此方法</remarks>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToEscapeRegChars(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            //[符号要第一个替换
            source = source.Replace("[", "[[]");

            source = source.Replace("%", "[%]");
            source = source.Replace("_", "[_]");
            source = source.Replace("^", "[^]");
            return source;
        }

        /// <summary>
        /// 将字符串包装成 &lt;![CDATA[字符串]]&gt; 形式
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string WrapWithCData(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            return string.Format("<![CDATA[{0}]]>", source);
        }

        /// <summary>
        /// 将字符串转换化安全的XML字符串值
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToSafeXmlString(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            return source.Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;").Replace("\"", "&quot;").Replace("'", "&apos;");
        }

        /// <summary>   
        /// 将字母，数字由全角转化为半角   
        /// </summary>   
        /// <returns></returns>   
        public static string NarrowToSmall(this string inputString)
        {
            char[] c = inputString.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                byte[] b = System.Text.Encoding.Unicode.GetBytes(c, i, 1);
                if (b.Length == 2)
                {
                    if (b[1] == 255)
                    {
                        b[0] = (byte)(b[0] + 32);
                        b[1] = 0;
                        c[i] = System.Text.Encoding.Unicode.GetChars(b)[0];
                    }
                }
            }
            string returnString = new string(c);
            return returnString;   // 返回半角字符   
        }

        /// <summary>   
        /// 将字母，数字由半角转化为全角   
        /// </summary>   
        /// <param name="inputString"></param>   
        /// <returns></returns>   
        public static string NarrowToBig(this string inputString)
        {
            char[] c = inputString.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                byte[] b = System.Text.Encoding.Unicode.GetBytes(c, i, 1);
                if (b.Length == 2)
                {
                    if (b[1] == 0)
                    {
                        b[0] = (byte)(b[0] - 32);
                        b[1] = 255;
                        c[i] = System.Text.Encoding.Unicode.GetChars(b)[0];
                    }
                }
            }
            string returnString = new string(c);
            return returnString;   // 返回全角字符   
        }
        #endregion

        #region 类型转换
        /// <summary>
        /// 将字符串转成Int32类型，如果转换失败，则返回-1
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns></returns>
        public static int ToInt32(this string source)
        {
            return source.ToInt32(-1);
        }
        /// <summary>
        /// 将字符串转成Int32类型
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="defaultValue">如果转换失败，返回的数值</param>
        /// <returns></returns>
        public static int ToInt32(this string source, int defaultValue)
        {
            if (!string.IsNullOrEmpty(source))
            {
                int result;
                if (Int32.TryParse(source, out result))
                {
                    return result;
                }
            }
            return defaultValue;
        }
        /// <summary>
        /// 将字符串转成Int64类型，如果转换失败，则返回-1
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns></returns>
        public static Int64 ToInt64(this string source)
        {
            return source.ToInt64(-1);
        }
        /// <summary>
        /// 将字符串转成Int64类型
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="defaultValue">如果转换失败，返回的数值</param>
        /// <returns></returns>
        public static Int64 ToInt64(this string source, Int64 defaultValue)
        {
            if (!string.IsNullOrEmpty(source))
            {
                Int64 result;
                if (Int64.TryParse(source, out result))
                {
                    return result;
                }
            }
            return defaultValue;
        }
        /// <summary>
        /// 将字符串转成double类型，如果转换失败，则返回-1
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns></returns>
        public static double ToDouble(this string source)
        {
            return source.ToDouble(-1.0);
        }
        /// <summary>
        /// 将字符串转成double类型
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="defaultValue">如果转换失败，返回的数值</param>
        /// <returns></returns>
        public static double ToDouble(this string source, double defaultValue)
        {
            if (!string.IsNullOrEmpty(source))
            {
                double result;
                if (Double.TryParse(source, out result))
                {
                    return result;
                }
            }
            return defaultValue;
        }
        /// <summary>
        /// 将字符串转成DateTime类型，如果转换失败，则返回当前时间
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string source)
        {
            return source.ToDateTime(DateTime.Now);
        }
        /// <summary>
        /// 将字符串转成DateTime类型
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="defaultValue">如果转换失败，返回的默认时间</param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string source, DateTime defaultValue)
        {
            if (!string.IsNullOrEmpty(source))
            {
                DateTime result;
                if (DateTime.TryParse(source, out result))
                {
                    return result;
                }
            }
            return defaultValue;
        }
        /// <summary>
        /// 将字符串转成Boolean类型，如果转换失败，则返回false
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns></returns>
        public static bool ToBoolean(this string source)
        {
            if (!string.IsNullOrEmpty(source))
            {
                Boolean result;
                if (Boolean.TryParse(source, out result))
                {
                    return result;
                }
            }
            return false;
        }
        /// <summary>
        /// 将字符串转成指定的枚举类型(字符串可以是枚举的名称也可以是枚举值)
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="source">源字符串</param>
        /// <param name="defaultValue">如果转换失败，返回默认的枚举项</param>
        /// <returns></returns>
        public static T ToEnum<T>(this string source, T defaultValue)
        {
            if (!string.IsNullOrEmpty(source))
            {
                try
                {
                    T value = (T)Enum.Parse(typeof(T), source, true);
                    if (Enum.IsDefined(typeof(T), value))
                    {
                        return value;
                    }
                }
                catch { }
            }
            return defaultValue;
        }

        /// <summary>
        /// 将字符串转成指定的枚举类型(字符串可以是枚举的名称也可以是枚举值)
        /// <remarks>支持枚举值的并集</remarks>
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="source">源字符串</param>
        /// <param name="defaultValue">如果转换失败，返回默认的枚举项</param>
        /// <returns></returns>
        public static T ToEnumExt<T>(this string source, T defaultValue)
        {
            if (!string.IsNullOrEmpty(source))
            {
                try
                {
                    return (T)Enum.Parse(typeof(T), source, true);
                }
                catch { }
            }
            return defaultValue;
        }
        #endregion

        /// <summary>
        /// 去除文件名中不可用于文件名的11个字符
        /// </summary>
        /// <param name="filenameNoDir"></param>
        /// <param name="replaceWith">用什么字符串替换</param>
        /// <returns></returns>
        public static string ReplaceNonValidChars(this string filenameNoDir, string replaceWith)
        {
            if (string.IsNullOrEmpty(filenameNoDir))
                return string.Empty;
            //替换这9个字符<>/\|:"*? 以及 回车换行
            return Regex.Replace(filenameNoDir, @"[\<\>\/\\\|\:""\*\?\r\n]", replaceWith, RegexOptions.Compiled);
        }

        /// <summary>
        /// 去除非打印字符
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string RemoveNonPrintChars(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            Regex reg = new Regex("[\x00-\x08\x0B\x0C\x0E-\x1F]");
            return reg.Replace(source, "");
        }

        /// <summary>
        /// 获取汉字字符串的首字母
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string GetPinYin(this string source)
        {
            return GetChineseSpell(source);
        }

        /// <summary>
        /// 取得汉字字符串的拼音的首字母
        /// </summary>
        /// <param name="strText"></param>
        /// <returns></returns>
        private static string GetChineseSpell(string strText)
        {
            if (string.IsNullOrEmpty(strText))
                return string.Empty;
            int len = strText.Length;
            string myStr = "";
            for (int i = 0; i < len; i++)
            {
                myStr += getSpell(strText.Substring(i, 1));
            }
            return myStr;
        }

        /// <summary>
        /// 取得汉字字符的拼音的首字母
        /// </summary>
        /// <param name="cnChar"></param>
        /// <returns></returns>
        private static string getSpell(string cnChar)
        {
            if (string.IsNullOrEmpty(cnChar))
                return string.Empty;
            byte[] arrCN = Encoding.Default.GetBytes(cnChar);
            if (arrCN.Length > 1)
            {
                int area = (short)arrCN[0];
                int pos = (short)arrCN[1];
                int code = (area << 8) + pos;
                int[] areacode = { 45217, 45253, 45761, 46318, 46826, 47010, 47297, 47614, 48119, 48119, 49062, 49324, 49896, 50371, 50614, 50622, 50906, 51387, 51446, 52218, 52698, 52698, 52698, 52980, 53689, 54481 };
                for (int i = 0; i < 26; i++)
                {
                    int max = 55290;
                    if (i != 25) max = areacode[i + 1];
                    if (areacode[i] <= code && code < max)
                    {
                        return Encoding.Default.GetString(new byte[] { (byte)(65 + i) });
                    }
                }
                return "*";
            }
            else return cnChar;
        }

        /// <summary>
        /// 字符串是否不为空
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNotNullOrEmpty(this string str)
        {
            return str != null && str != string.Empty && str != "";
        }
        /// <summary>
        /// 判断是否是大于或等于0的整数
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsNumeric(this string input)
        {
            int output = 0;
            return int.TryParse(input, out output);
        }

        /// <summary>
        /// 判断字符串是否是Decimal
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsDecimal(this string input)
        {
            Decimal output = 0;
            return decimal.TryParse(input, out output);
        }
        /// <summary>
        /// 判断对象是否是Boolean
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsBoolean(this string input)
        {
            bool output = false;
            return Boolean.TryParse(input, out output);
        }
        /// <summary>
        /// 判断是否是合法的QQ号码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsQQ(this string input)
        {
            Regex reg = new Regex(@"^([1-9][0-9]{4,10})$");
            return reg.IsMatch(input);
        }
        /// <summary>
        /// 字符串是否存在空格或者单引号字符
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsExistIllegalChar(this string input)
        {
            if (input.IndexOf(' ') >= 0)
                return true;
            if (input.IndexOf('\'') >= 0)
                return true;
            return false;
        }
        /// <summary>
        /// 是否是32位MD5
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsMD5(this string input)
        {
            Regex reg = new Regex(@"^([\w\d]{32})$");
            return reg.IsMatch(input);
        }

        /// <summary>
        /// 判断是否是中国邮政编码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsZipCode(this string input)
        {
            Regex reg = new Regex(@"^([\d]{6})$");
            return reg.IsMatch(input);
        }
        /// <summary>
        /// 是否是手机号码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsMobilPhone(this string input)
        {
            Regex reg = new Regex(@"^([\d]{11})$");
            return reg.IsMatch(input);
        }
        /// <summary>
        /// 是否是电话号码  格式  区号-电话号码  或者是 7-8位电话号码 或者是 区号 空格 7-8位电话号码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsTelPhone(this string input)
        {
            Regex reg = new Regex(@"^(([\d]{3,4})?([-\s]?)[\d]{7,8})$");
            return reg.IsMatch(input);
        }

        /// <summary>
        /// 判断是否存在特殊字符
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsExistSpecialChar(this string input)
        {
            Regex reg = new Regex(@"[-`=\\\[\];',./~!@#$%^&*()_+|{}:" + "\"<>?]");
            MatchCollection matches = reg.Matches(input);
            if (matches.Count > 0)
                return true;
            return false;
        }
        /// <summary>
        /// 判断是否存在数字
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsExistNumber(this string input)
        {
            Regex reg = new Regex(@"\d");
            MatchCollection matches = reg.Matches(input);
            if (matches.Count > 0)
                return true;
            return false;
        }
        /// <summary>
        /// 判断是否存在字母
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsExistLetters(this string input)
        {
            Regex reg = new Regex(@"[a-zA-Z]");
            MatchCollection matches = reg.Matches(input);
            if (matches.Count > 0)
                return true;
            return false;
        }

        /// <summary>
        /// 将对象转化为Int32类型
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int GetInt(this object input)
        {
            return Convert.ToInt32(input);
        }
        /// <summary>
        /// 将对象转化为Int64类型
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static long GetLong(this object input)
        {
            return Convert.ToInt64(input);
        }
        /// <summary>
        /// 将对象转化为DateTime类型
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DateTime GetDateTime(this object input)
        {
            return Convert.ToDateTime(input);
        }
        /// <summary>
        /// 将对象转化为DateTime类型
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DateTime? GetDateTimeNullable(this object input)
        {
            if (input == null || input.ToString().IsNullOrEmpty())
                return null;
            return Convert.ToDateTime(input);
        }
        /// <summary>
        /// 将对象转化为Decimal类型
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Decimal GetDecimal(this object input)
        {
            return Convert.ToDecimal(input);
        }
        /// <summary>
        /// 将对象转化为Boolean类型
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool GetBool(this object input)
        {
            return Convert.ToBoolean(input);
        }
        /// <summary>
        /// 截取字符串的字节长度，超出部分以...代替
        /// </summary>
        /// <param name="input"></param>
        /// <param name="byteNum">保留的字节长度(不包含省略号)</param>
        /// <returns></returns>
        public static string SubStringByBytes(this string input, int byteNum)
        {
            string result = string.Empty;// 最终返回的结果
            int byteLen = System.Text.Encoding.Default.GetByteCount(input);// 单字节字符长度
            int charLen = input.Length;// 把字符平等对待时的字符串长度
            int byteCount = 0;// 记录读取进度
            int pos = 0;// 记录截取位置
            if (byteLen > byteNum)
            {
                for (int i = 0; i < charLen; i++)
                {
                    if (Convert.ToInt32(input.ToCharArray()[i]) > 255)// 按中文字符计算加2
                        byteCount += 2;
                    else// 按英文字符计算加1
                        byteCount += 1;

                    if (byteCount > byteNum)// 超出时只记下上一个有效位置
                    {
                        pos = i;
                        break;
                    }
                    else if (byteCount == byteNum)// 记下当前位置
                    {
                        pos = i + 1;
                        break;
                    }
                }

                if (pos >= 0)
                    result = input.Substring(0, pos - 3) + "...";
            }
            else
                result = input;

            return result;
        }
        /// <summary>
        /// 将数字字符串每隔4位加上逗号
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string ConcatWithComma(this string num)
        {
            string newstr = string.Empty;
            Regex r = new Regex(@"(\d+?)(\d{4})*(\.\d+|$)");
            Match m = r.Match(num);
            newstr += m.Groups[1].Value;
            for (int i = 0; i < m.Groups[2].Captures.Count; i++)
            {
                newstr += "," + m.Groups[2].Captures[i].Value;
            }
            newstr += m.Groups[3].Value;
            return newstr;
        }
    }
}