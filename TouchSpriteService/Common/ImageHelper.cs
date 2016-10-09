using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouchSpriteService.Common
{
    public static class ImageHelper
    {
        /// <summary>
        /// Convert Image to Byte[]
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static byte[] ImageToBytes(Image image)
        {
            ImageFormat format = image.RawFormat;
            using (MemoryStream ms = new MemoryStream())
            {
                if (format.Equals(ImageFormat.Jpeg))
                {
                    image.Save(ms, ImageFormat.Jpeg);
                }
                else if (format.Equals(ImageFormat.Png))
                {
                    image.Save(ms, ImageFormat.Png);
                }
                else if (format.Equals(ImageFormat.Bmp))
                {
                    image.Save(ms, ImageFormat.Bmp);
                }
                else if (format.Equals(ImageFormat.Gif))
                {
                    image.Save(ms, ImageFormat.Gif);
                }
                else if (format.Equals(ImageFormat.Icon))
                {
                    image.Save(ms, ImageFormat.Icon);
                }
                byte[] buffer = new byte[ms.Length];
                //Image.Save()会改变MemoryStream的Position，需要重新Seek到Begin
                ms.Seek(0, SeekOrigin.Begin);
                ms.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }

        /// <summary>
        /// Convert Byte[] to Image
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static Image BytesToImage(byte[] buffer)
        {
            MemoryStream ms = new MemoryStream(buffer);
            Image image = System.Drawing.Image.FromStream(ms);
            return image;
        }

        /// <summary>
        /// Convert Byte[] to a picture and Store it in file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static string CreateImageFromBytes(string fileName, byte[] buffer)
        {
            string file = fileName;
            Image image = BytesToImage(buffer);
            ImageFormat format = image.RawFormat;
            if (format.Equals(ImageFormat.Jpeg))
            {
                file += ".jpeg";
            }
            else if (format.Equals(ImageFormat.Png))
            {
                file += ".png";
            }
            else if (format.Equals(ImageFormat.Bmp))
            {
                file += ".bmp";
            }
            else if (format.Equals(ImageFormat.Gif))
            {
                file += ".gif";
            }
            else if (format.Equals(ImageFormat.Icon))
            {
                file += ".icon";
            }
            System.IO.FileInfo info = new System.IO.FileInfo(file);
            System.IO.Directory.CreateDirectory(info.Directory.FullName);
            File.WriteAllBytes(file, buffer);
            return file;
        }

        public static byte[] BytesFromImageFile(string fileName)
        {
            byte[] fileBytes = new byte[0];
            FileInfo file = new FileInfo(fileName);
            if (!file.Exists)
            {
                return fileBytes;
            }
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            fileBytes = new byte[fs.Length];
            fs.Read(fileBytes, 0, Convert.ToInt32(fs.Length));
            return fileBytes;
        }

        #region 压缩图片
        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static byte[] PercentImage(byte[] bytes, double percent)
        {
            Image image = BytesToImage(bytes);
            Bitmap bitmap = PercentImage(image, percent);
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            byte[] buffer = ms.GetBuffer();
            ms.Close();
            return buffer;
        }

        /// <summary>
        /// 压缩图片  压缩比0.2D时候显示正常，其他时候显示图片错误；
        /// </summary>
        /// <param name="srcImage"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static Bitmap PercentImage(Image srcImage, double percent)
        {
            // 缩小后的高度 
            int newH = int.Parse(Math.Round(srcImage.Height * percent).ToString());
            // 缩小后的宽度 
            int newW = int.Parse(Math.Round(srcImage.Width * percent).ToString());
            try
            {
                // 要保存到的图片 
                Bitmap b = new Bitmap(newW, newH);
                Graphics g = Graphics.FromImage(b);
                // 插值算法的质量 
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
                g.DrawImage(srcImage, new Rectangle(0, 0, newW, newH), new Rectangle(0, 0, srcImage.Width, srcImage.Height), GraphicsUnit.Pixel);
                g.Dispose();
                return b;
            }
            catch (Exception)
            {
                return null;
            }
        }


        public static byte[] GetReducedImage(byte[] bytes, int Width, int Height)
        {
            Image image = BytesToImage(bytes);
            Image bitmap = GetReducedImage(image, Width, Height);
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            byte[] buffer = ms.GetBuffer();
            ms.Close();
            return buffer;
        }

        /// <summary>
        /// 获得缩略图
        /// </summary>
        /// <param name="Resource"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <returns></returns>
        public static Image GetReducedImage(Image Resource,int Width, int Height)
        {
            try
            {
                //用指定的大小和格式初始化Bitmap类的新实例
                Bitmap bitmap = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
                //从指定的Image对象创建新Graphics对象
                Graphics graphics = Graphics.FromImage(bitmap);
                //清除整个绘图面并以透明背景色填充
                graphics.Clear(Color.Transparent);
                //在指定位置并且按指定大小绘制原图片对象
                graphics.DrawImage(Resource, new Rectangle(0, 0, Width, Height));
                return bitmap;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion
    }
}
