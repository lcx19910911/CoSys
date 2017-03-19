using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoSys.Core
{
    public class NPOIHelper<T>
    {
        public NPOIHelper()
        {
        }
        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="lists"></param>
        /// <param name="head">中文列名对照</param>
        /// <param name="workbookFile">保存路径</param>
        public static void GetExcel(List<T> lists, Hashtable head, string workbookFile)
        {
            try
            {
                HSSFWorkbook workbook = new HSSFWorkbook();
                MemoryStream ms = new MemoryStream();
                HSSFSheet sheet = workbook.CreateSheet() as HSSFSheet;
                HSSFRow headerRow = sheet.CreateRow(0) as HSSFRow;
                bool h = false;
                int j = 1;
                Type type = typeof(T);
                PropertyInfo[] properties = type.GetProperties();

                foreach (T item in lists)
                {
                    HSSFRow dataRow = sheet.CreateRow(j) as HSSFRow;
                    int i = 0;
                    foreach (PropertyInfo column in properties)
                    {
                        if (!h)
                        {
                            if (head.ContainsKey(column.Name))
                            {
                                headerRow.CreateCell(i).SetCellValue(head[column.Name] == null ? column.Name : head[column.Name].ToString());
                                dataRow.CreateCell(i).SetCellValue(column.GetValue(item, null) == null ? "" : column.GetValue(item, null).ToString());
                                i++;
                            }
                        }
                        else
                        {
                            if (head.ContainsKey(column.Name))
                            {
                                dataRow.CreateCell(i).SetCellValue(column.GetValue(item, null) == null ? "" : column.GetValue(item, null).ToString());
                                i++;
                            }
                        }
                    }
                    h = true;
                    j++;
                }
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;
                sheet = null;
                headerRow = null;
                workbook = null;
                FileStream fs = new FileStream(workbookFile, FileMode.Create, FileAccess.Write);
                byte[] data = ms.ToArray();
                fs.Write(data, 0, data.Length);
                fs.Flush();
                fs.Close();
                data = null;
                ms = null;
                fs = null;
            }
            catch (Exception ee)
            {
                string see = ee.Message;
            }
        }
        /// <summary>
        /// 导入Excel
        /// </summary>
        /// <param name="lists"></param>
        /// <param name="head">中文列名对照</param>
        /// <param name="workbookFile">Excel所在路径</param>
        /// <returns></returns>
        public static List<T> FromExcel(Hashtable head, string workbookFile)
        {
            try
            {
                HSSFWorkbook hssfworkbook;
                List<T> lists = new List<T>();
                using (FileStream file = new FileStream(workbookFile, FileMode.Open, FileAccess.Read))
                {
                    hssfworkbook = new HSSFWorkbook(file);
                }
                HSSFSheet sheet = hssfworkbook.GetSheetAt(0) as HSSFSheet;
                IEnumerator rows = sheet.GetRowEnumerator();
                HSSFRow headerRow = sheet.GetRow(0) as HSSFRow;
                int cellCount = headerRow.LastCellNum;
                //Type type = typeof(T);
                PropertyInfo[] properties;
                T t = default(T);
                for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
                {
                    HSSFRow row = sheet.GetRow(i) as HSSFRow;
                    t = Activator.CreateInstance<T>();
                    properties = t.GetType().GetProperties();
                    foreach (PropertyInfo column in properties)
                    {
                        int j = headerRow.Cells.FindIndex(delegate (ICell c)
                        {
                            return c.StringCellValue == (head[column.Name] == null ? column.Name : head[column.Name].ToString());
                        });
                        if (j >= 0 && row.GetCell(j) != null)
                        {
                            object value = valueType(column.PropertyType, row.GetCell(j).ToString());
                            column.SetValue(t, value, null);
                        }
                    }
                    lists.Add(t);
                }
                return lists;
            }
            catch (Exception ee)
            {
                string see = ee.Message;
                return null;
            }
        }
        static object valueType(Type t, string value)
        {
            object o = null;
            string strt = "String";
            if (t.Name == "Nullable`1")
            {
                strt = t.GetGenericArguments()[0].Name;
            }
            switch (strt)
            {
                case "Decimal":
                    o = decimal.Parse(value);
                    break;
                case "Int":
                    o = int.Parse(value);
                    break;
                case "Float":
                    o = float.Parse(value);
                    break;
                case "DateTime":
                    o = DateTime.Parse(value);
                    break;
                default:
                    o = value;
                    break;
            }
            return o;
        }
    }

}
