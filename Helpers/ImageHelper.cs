using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace game_tools.Helpers
{
    public static class ImageHelper
    {
        public static void SaveImageBytesToFile(Bitmap bitmap, string fileName, string folderPath)
        {
            try
            {
                // 确保目录存在
                Directory.CreateDirectory(folderPath);

                // 构造文件路径
                string filePath = Path.Combine(folderPath, fileName);

                // 保存位图到指定路径
                bitmap.Save(filePath);

                Console.WriteLine("Image saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving image: {ex.Message}");
            }
            finally
            {
                // 释放资源
                bitmap.Dispose();
            }
        }

        public static byte[] LoadImageAsByteArray(string imagePath)
        {
            // Step 1: Load the image using BitmapImage
            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, imagePath);

            // 读取图片文件到 byte[]
            byte[] imageBytes = File.ReadAllBytes(path);

            return imageBytes;
        }
    }
}
