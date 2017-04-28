using CoSys.Model;
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CoSys.Core
{
    public class WordHelper
    {
        /// <summary>      
        /// 取得HTML中所有图片的 URL。      
        /// </summary>      
        /// <param name="sHtmlText">HTML代码</param>      
        /// <returns>图片的URL列表</returns>      
        public static string[] GetHtmlImageUrlList(string sHtmlText)
        {
            // 定义正则表达式用来匹配 img 标签      
            Regex regImg = new Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", RegexOptions.IgnoreCase);

            // 搜索匹配的字符串      
            MatchCollection matches = regImg.Matches(sHtmlText);
            int i = 0;
            string[] sUrlList = new string[matches.Count];

            // 取得匹配项列表      
            foreach (Match match in matches)
                sUrlList[i++] = match.Groups["imgUrl"].Value;
            return sUrlList;
        }

        public static MemoryStream Export(News model, string plsushMethodStr)
        {

            XWPFDocument doc = new XWPFDocument();

            XWPFParagraph p1 = doc.CreateParagraph();
            XWPFRun r1 = p1.CreateRun();
            r1.FontSize = 18;
            r1.FontFamily = "微软雅黑";
            r1.SetText(model.Title);
            r1.SetTextPosition(30);

            CT_P doc_p1 = doc.Document.body.GetPArray(0);//标题居中
            doc_p1.AddNewPPr().AddNewJc().val = ST_Jc.center;


            XWPFParagraph p2 = doc.CreateParagraph();
            //段落对其方式为居中  
            p2.Alignment = ParagraphAlignment.RIGHT;
            XWPFRun r2 = p2.CreateRun();//向该段落中添加文字  
            r2.FontSize = 8;//设置大小  
            r2.FontFamily = "微软雅黑";
            r2.SetText($"作者:{model.PenName}");


            XWPFParagraph p3 = doc.CreateParagraph();
            //段落对其方式为居中  
            p3.Alignment = ParagraphAlignment.RIGHT;
            XWPFRun r3 = p3.CreateRun();//向该段落中添加文字  
            r3.FontSize = 8;//设置大小  
            r3.FontFamily = "微软雅黑";
            r3.SetText($"投稿时间:{model.SubmitTime}");

            if (plsushMethodStr != "")
            {
                XWPFParagraph p4 = doc.CreateParagraph();
                //段落对其方式为居中  
                p4.Alignment = ParagraphAlignment.RIGHT;
                XWPFRun r4 = p4.CreateRun();//向该段落中添加文字  
                r4.FontSize = 8;//设置大小  
                r4.FontFamily = "微软雅黑";
                r4.SetText($"发布渠道:{plsushMethodStr}");
            }


            var list=GetHtmlImageUrlList(model.Content);

            if (list.Length > 0)
            {
                foreach (var item in list)
                {
                    model.Content.Replace(item, "❶");
                }
                var ary = model.Content.Split('❶');
                if (ary.Length > 0)
                {
                    for (var i= 0; i < ary.Length;i++)
                    {
                        XWPFParagraph obj = doc.CreateParagraph();
                        //段落对其方式为居中  
                        obj.Alignment = ParagraphAlignment.LEFT;
                        XWPFRun textRun = obj.CreateRun();//向该段落中添加文字  
                        textRun.FontSize = 10;//设置大小  
                        textRun.FontFamily = "微软雅黑";
                        textRun.SetText($"{(char)9}{xxHTML(ary[i])}");

                        XWPFRun r6 = obj.CreateRun();//向该段落中添加文字  

                        var widthEmus = (int)(400.0 * 9525);
                        var heightEmus = (int)(300.0 * 9525);

                        using (FileStream picData = new FileStream(Get_img(list[i]), FileMode.Open, FileAccess.Read))
                        {
                            r6.AddPicture(picData, (int)PictureType.PNG, "图片", widthEmus, heightEmus);
                        }
                    }
                }
            }
            else
            {
                XWPFParagraph p5 = doc.CreateParagraph();
                //段落对其方式为居中  
                p5.Alignment = ParagraphAlignment.LEFT;
                XWPFRun r5 = p5.CreateRun();//向该段落中添加文字  
                r5.FontSize = 10;//设置大小  
                r5.FontFamily = "微软雅黑";
                r5.SetText($"{(char)9}{xxHTML(model.Content)}");
            }

            if (model.Paths.IsNotNullOrEmpty())
            {
                XWPFParagraph p6 = doc.CreateParagraph();
                //段落对其方式为居中  
                p6.Alignment = ParagraphAlignment.LEFT;
                XWPFRun r6 = p6.CreateRun();//向该段落中添加文字  

                var widthEmus = (int)(400.0 * 9525);
                var heightEmus = (int)(300.0 * 9525);
                var index = 1;
                foreach (var item in model.Paths.Split(','))
                {

                    using (FileStream picData = new FileStream(System.Web.HttpContext.Current.Request.PhysicalApplicationPath + item, FileMode.Open, FileAccess.Read))
                    {
                        r6.AddPicture(picData, (int)PictureType.PNG, "图片" + index, widthEmus, heightEmus);
                    }
                    index++;
                }
            }

            return new MemoryStream(ToByte(doc));

        }

        private static byte[] ToByte(XWPFDocument wb)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                //XSSFWorkbook即读取.xlsx文件返回的MemoryStream是关闭
                //但是可以ToArray(),这是NPOI的bug
                wb.Write(ms);
                return ms.ToArray();
            }
        }

        public static string Get_img(string url)
        {
            Bitmap img = null;
            HttpWebRequest req;
            HttpWebResponse res = null;
            string path = "";
            try
            {
                System.Uri httpUrl = new System.Uri(url);
                req = (HttpWebRequest)(WebRequest.Create(httpUrl));
                req.Timeout = 180000; //设置超时值10秒
                req.UserAgent = "XXXXX";
                req.Accept = "XXXXXX";
                req.Method = "GET";
                res = (HttpWebResponse)(req.GetResponse());
                img = new Bitmap(res.GetResponseStream());//获取图片流          
                path = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + @"/Save/" + Guid.NewGuid().ToString("N") + ".png";
                img.Save(path);//随机名
            }

            catch (Exception ex)
            {
                string aa = ex.Message;
            }
            finally
            {
                res.Close();
            }
            return path;
        }
        public static string xxHTML(string html)
        {

            html = html.Replace("(<style)+[^<>]*>[^\0]*(</style>)+", "");
            html = html.Replace(@"\<img[^\>] \>", "");
            html = html.Replace(@"<p>", "");
            html = html.Replace(@"</p>", "");


            System.Text.RegularExpressions.Regex regex0 =
            new System.Text.RegularExpressions.Regex("(<style)+[^<>]*>[^\0]*(</style>)+", System.Text.RegularExpressions.RegexOptions.Multiline);
            System.Text.RegularExpressions.Regex regex1 = new System.Text.RegularExpressions.Regex(@"<script[\s\S] </script *>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex2 = new System.Text.RegularExpressions.Regex(@" href *= *[\s\S]*script *:", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex3 = new System.Text.RegularExpressions.Regex(@" on[\s\S]*=", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex4 = new System.Text.RegularExpressions.Regex(@"<iframe[\s\S] </iframe *>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex5 = new System.Text.RegularExpressions.Regex(@"<frameset[\s\S] </frameset *>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex6 = new System.Text.RegularExpressions.Regex(@"\<img[^\>] \>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex7 = new System.Text.RegularExpressions.Regex(@"</p>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex8 = new System.Text.RegularExpressions.Regex(@"<p>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex9 = new System.Text.RegularExpressions.Regex(@"<[^>]*>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            html = regex1.Replace(html, ""); //过滤<script></script>标记  
            html = regex2.Replace(html, ""); //过滤href=javascript: (<A>) 属性   
            html = regex0.Replace(html, ""); //过滤href=javascript: (<A>) 属性   


            //html = regex10.Replace(html, "");  
            html = regex3.Replace(html, "");// _disibledevent="); //过滤其它控件的on...事件  
            html = regex4.Replace(html, ""); //过滤iframe  
            html = regex5.Replace(html, ""); //过滤frameset  
            html = regex6.Replace(html, ""); //过滤frameset  
            html = regex7.Replace(html, ""); //过滤frameset  
            html = regex8.Replace(html, ""); //过滤frameset  
            html = regex9.Replace(html, "");
            //html = html.Replace(" ", "");  
            html = html.Replace("</strong>", "");
            html = html.Replace("<strong>", "");
            html = html.Replace(" ", "");
            return html;
        }
    }
}
