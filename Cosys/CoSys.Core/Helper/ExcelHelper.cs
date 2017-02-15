using System;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;

namespace CoSys.Core
{
    /// <summary>
    /// Excel 助手类。
    /// </summary>
    public class ExcelHelper<T>
    {
        /// <summary>  
        /// 导出Excel  
        /// </summary>  
        /// <param name="lists"></param>  
        /// <param name="head">中文列名对照</param>  
        /// <param name="workbookFile">保存路径</param>  
        public static void getExcel(List<T> lists, Hashtable head, string workbookFile)
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
                            headerRow.CreateCell(i).SetCellValue(head[column.Name] == null ? column.Name : head[column.Name].ToString());
                            dataRow.CreateCell(i).SetCellValue(column.GetValue(item, null) == null ? "" : column.GetValue(item, null).ToString());
                        }
                        else
                        {
                            dataRow.CreateCell(i).SetCellValue(column.GetValue(item, null) == null ? "" : column.GetValue(item, null).ToString());
                        }

                        i++;
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
        /// 导出Excel  
        /// </summary>  
        /// <param name="lists"></param>  
        /// <param name="head">中文列名对照</param>  
        /// <param name="workbookFile">保存路径</param>  
        public static void getDrinkExcel(List<T> lists, Hashtable head, string workbookFile)
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
                            headerRow.CreateCell(i).SetCellValue(head[column.Name] == null ? column.Name : head[column.Name].ToString());
                            dataRow.CreateCell(i).SetCellValue(column.GetValue(item, null) == null ? "" : column.GetValue(item, null).ToString());
                        }
                        else
                        {
                            dataRow.CreateCell(i).SetCellValue(column.GetValue(item, null) == null ? "" : column.GetValue(item, null).ToString());
                        }

                        i++;
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
    }
}
