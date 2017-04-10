using CoSys.Model;
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoSys.Core
{
    public class WordHelper
    {
        public static MemoryStream Export(News model,string plsushMethodStr)
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
            XWPFParagraph p5 = doc.CreateParagraph();
            //段落对其方式为居中  
            p5.Alignment = ParagraphAlignment.LEFT;
            XWPFRun r5 = p5.CreateRun();//向该段落中添加文字  
            r5.FontSize = 10;//设置大小  
            r5.FontFamily = "微软雅黑";
            r5.SetText($"{(char)9}{model.Content}");

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

                    using (FileStream picData = new FileStream(item, FileMode.Open, FileAccess.Read))
                    {
                        r6.AddPicture(picData, (int)PictureType.PNG, "图片"+index, widthEmus, heightEmus);
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
    }
}
