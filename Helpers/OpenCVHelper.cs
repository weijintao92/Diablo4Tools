using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D11.Device;
using MapFlags = SharpDX.Direct3D11.MapFlags;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.Security.Cryptography.Xml;
using System.Windows.Resources;
using System.Windows.Controls;


namespace game_tools.Helpers
{
    public static class OpenCVHelper
    {
        public static System.Drawing.Point FindImage(Bitmap haystack)
        {
            // 加载要查找的图像
            Bitmap needleImage = LoadBitmapFromResource("Resources/topLeft_1080.png");

            Image<Bgr, byte> source = haystack.ToImage<Bgr, byte>();
            Image<Bgr, byte> template = needleImage.ToImage<Bgr, byte>();

            using (var result = source.MatchTemplate(template, TemplateMatchingType.CcoeffNormed))
            {
                double[] minValues, maxValues;
                System.Drawing.Point[] minLocations, maxLocations;
                result.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);

                if (maxValues[0] > 0.7) // 设定一个阈值
                {
                    return maxLocations[0]; // 返回找到的图像位置
                }
            }

            return new System.Drawing.Point(); // 没有找到匹配的图像
        }

        public static System.Drawing.Point FindStar(Bitmap haystack)
        {
            // 加载要查找的图像
            Bitmap needleImage = LoadBitmapFromResource("Resources/star_1080.png");

            Image<Bgr, byte> source = haystack.ToImage<Bgr, byte>();
            Image<Bgr, byte> template = needleImage.ToImage<Bgr, byte>();

            using (var result = source.MatchTemplate(template, TemplateMatchingType.CcoeffNormed))
            {
                double[] minValues, maxValues;
                System.Drawing.Point[] minLocations, maxLocations;
                result.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);

                if (maxValues[0] > 0.8) // 设定一个阈值
                {
                    return maxLocations[0]; // 返回找到的图像位置
                }
            }

            return new System.Drawing.Point(); // 没有找到匹配的图像
        }


        private static Bitmap LoadBitmapFromResource(string resourcePath)
        {
            try
            {
                // 构建资源 URI
                Uri resourceUri = new Uri($"pack://application:,,,/{resourcePath}", UriKind.Absolute);

                // 获取资源流
                StreamResourceInfo resourceInfo = Application.GetResourceStream(resourceUri);

                if (resourceInfo != null)
                {
                    // 创建 Bitmap 对象
                    return new Bitmap(resourceInfo.Stream);
                }
                else
                {
                    throw new FileNotFoundException("资源文件未找到", resourcePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载图片时发生错误: {ex.Message}");
                return null;
            }
        }

        // 异步截屏方法
        public static Bitmap CaptureScreenAsync(int x, int y, int width, int height)
        {
      
                Bitmap bitmap = new Bitmap(width, height);
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(x, y, 0, 0, new System.Drawing.Size(width, height), CopyPixelOperation.SourceCopy);
                }
                return bitmap;
        }

        public static byte[] BitmapToByteArray(Bitmap bitmap)
        {
            // 使用内存流来保存Bitmap数据
            using (MemoryStream memoryStream = new MemoryStream())
            {
                // 将Bitmap保存到内存流中
                bitmap.Save(memoryStream, ImageFormat.Png); // 这里可以根据需要选择不同的图像格式
                                                            // 将内存流转换为字节数组
                return memoryStream.ToArray();
            }
        }
        public static Bitmap CropBitmapBelowY(Bitmap originalBitmap, int y)
        {
            // 检查输入的 y 是否超出图片的高度范围
            if (y >= originalBitmap.Height)
            {
                throw new ArgumentOutOfRangeException(nameof(y), "Y coordinate is out of bounds.");
            }

            // 指定截取的位置和尺寸
            int x = 0;
            int width = originalBitmap.Width;
            int height = originalBitmap.Height - y; // 计算截取的高度

            // 创建一个新的 Bitmap 对象来存储截取的部分
            System.Drawing.Rectangle cropRect = new System.Drawing.Rectangle(x, y, width, height);
            Bitmap croppedBitmap = originalBitmap.Clone(cropRect, originalBitmap.PixelFormat);

            return croppedBitmap;
        }

        public static void SaveBitmapToFile(Bitmap bitmap, string filePath = "captured_targe.png")
        {
            bitmap.Save(filePath, ImageFormat.Png);
        }

        // 异步保存Bitmap到文件
        public static Task SaveBitmapAsync(Bitmap bitmap, string filePath = "captured_targe.png")
        {
            return Task.Run(() =>
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    bitmap.Save(memoryStream, ImageFormat.Png);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        memoryStream.CopyTo(fileStream);
                    }
                }
            });
        }
    }
}
