using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace CoSys.Core
{
    public class ImageThumbnailHelper
    {
        public static Image ResourceImage;
        private static int ImageWidth;
        private static int ImageHeight;
        public static string ErrorMessage;

        public static bool ThumbnailCallback()
        {
            return false;
        }


        // 方法1，按大小
        public static bool ReducedImage(Stream stream, int Width, int Height, string targetFilePath)
        {
            try
            {
                ErrorMessage = "";
                ResourceImage = Image.FromStream(stream);
                Image ReducedImage;
                Image.GetThumbnailImageAbort callb = new Image.GetThumbnailImageAbort(ThumbnailCallback);
                ReducedImage = ResourceImage.GetThumbnailImage(Width, Height, callb, IntPtr.Zero);
                ReducedImage.Save(@targetFilePath, ImageFormat.Jpeg);
                ReducedImage.Dispose();
                return true;
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                return false;
            }
        }


        // 方法2，按百分比  缩小60% Percent为0.6 targetFilePath为目标路径
        public static bool ReducedImage(Stream stream,double Percent, string targetFilePath)
        {
            try
            {
                ErrorMessage = "";
                ResourceImage = Image.FromStream(stream);
                Image ReducedImage;
                Image.GetThumbnailImageAbort callb = new Image.GetThumbnailImageAbort(ThumbnailCallback);
                ImageWidth = Convert.ToInt32(ResourceImage.Width * Percent);
                ImageHeight = (ResourceImage.Height) * ImageWidth / ResourceImage.Width;//等比例缩放
                ReducedImage = ResourceImage.GetThumbnailImage(ImageWidth, ImageHeight, callb, IntPtr.Zero);
                ReducedImage.Save(targetFilePath, ImageFormat.Jpeg);
                ReducedImage.Dispose();
                return true;
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                return false;
            }
        }

    }
}
