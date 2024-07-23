using GalaSoft.MvvmLight.Messaging;
using game_tools.ViewModel;
using Newtonsoft.Json.Linq;
using PaddleOCRSharp;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;


namespace game_tools.Helpers
{
    public class PaddleOCRHelp
    {
        public PaddleOCREngine engine;
        private readonly SQLiteDb sQLiteDb;
        public PaddleOCRHelp(SQLiteDb sQLiteDb)
        {
            engine = CreateOCRParameter();// 这个只能引用一次，否则会出现内存一直增加的问题
            this.sQLiteDb = sQLiteDb;
        }

        public PaddleOCREngine CreateOCRParameter()
        {
            OCRParameter oCRParameter = new OCRParameter();
            //oCRParameter.cls = false; //是否执行文字方向分类；默认false
            //oCRParameter.det = false;//是否开启方向检测，用于检测识别180旋转
            //oCRParameter.use_angle_cls = false;//是否开启方向检测，用于检测识别180旋转
            //oCRParameter.det_db_score_mode = false;//是否使用多段线，即文字区域是用多段线还是用矩形，
            //oCRParameter.show_img_vis = true;//是否显示预测结果
            //oCRParameter.use_angle_cls = true;//是否使用方向分类器
            //oCRParameter.visualize = true;//是否对结果进行可视化，为true时，预测结果会在当前目录下保存一个ocr_vis.png文件。默认false
            //oCRParameter.cls = true; //是否执行文字方向分类

            OCRModelConfig config = null;
            PaddleOCREngine engine = new PaddleOCREngine(config, oCRParameter);
            return engine;
        }


        /// <summary>
        /// 保存识别结果
        /// </summary>
        /// <param name="selectionRectangle"></param>
        public void RecognizeAndSave(Bitmap bitmap, int index = 0,int taskBatch=0)
        {
            JObject jsonObject = new JObject();

            jsonObject = Recognize(bitmap);

            // 获取当前时间并格式化为字符串
            string imageName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".png";
            string folderPath = @"picture";
            string imagePth = Path.Combine(folderPath, imageName);
            ImageHelper.SaveImageBytesToFile(bitmap, imageName, folderPath);
            jsonObject.Add("ImagePath", imagePth);
            jsonObject.Add("ItemIndex", index);
            jsonObject.Add("TaskBatch", taskBatch);

            Equipments equipmentResult = jsonObject.ToObject<Equipments>();

            this.sQLiteDb.Equipments.Add(equipmentResult);
            this.sQLiteDb.SaveChanges();
            Messenger.Default.Send(new HomePageRefreshMessage());
        }

        /// <summary>
        /// 识别文字
        /// </summary>
        /// <param name="selectionRectangle"></param>
        public JObject Recognize(Bitmap bitmap)
        {
            List<EquipmentItems> equipmentItems = DataInitializer.initEquitpmentItems();
            JObject jsonObject = new JObject();

            OCRResult ocrResult = engine.DetectText(OpenCVHelper.BitmapToByteArray(bitmap));

            //威能
            System.Drawing.Point point = OpenCVHelper.FindStar(bitmap);
            bool isEmpty = point == System.Drawing.Point.Empty;

            try
            {
                for (int i = 2; i < ocrResult.TextBlocks.ToArray().Length; i++)
                {
                    var block = ocrResult.TextBlocks[i];
                    //获取当前行字符串
                    string mainString = block.Text;

                    //获取威能
                    var pointTopLeft = block.BoxPoints[0];
                    if (!isEmpty && Math.Abs(pointTopLeft.Y - point.Y) < 5)
                    {
                        string amazingPower = ocrResult.TextBlocks[i].Text;
                        string pattern = @"需要等级";
                        bool containsRequiredLevel = false;
                        var count = 1;

                        while (i + count < ocrResult.TextBlocks.ToArray().Length)
                        {
                            containsRequiredLevel = Regex.IsMatch(ocrResult.TextBlocks[i + count].Text, pattern);
                            if (containsRequiredLevel || ocrResult.TextBlocks[i + count].Text == "空插槽")
                            {
                                break;
                            }
                            amazingPower += ocrResult.TextBlocks[i + count].Text;
                            count++;
                        }
                        //jsonObject.Add("AmazingPower", amazingPower);
                        jsonObject["AmazingPower"] = amazingPower;

                    }

                    foreach (EquipmentItems item in equipmentItems)
                    {
                        bool isOk = Regex.IsMatch(mainString, Regex.Escape(item.subString));
                        if (isOk)
                        {
                            //jsonObject.Add(item.Name.ToString(), ExtractFirstNumber(mainString));
                            jsonObject[item.Name] = ExtractFirstNumber(mainString);
                            //获取装备名字、装备部位
                            if (item.Name == "Strength")
                            {
                                if (i == 2)
                                {
                                    //jsonObject.Add("Place", ocrResult.TextBlocks[1].Text);
                                    //jsonObject.Add("Name", ocrResult.TextBlocks[0].Text);
                                    jsonObject["Place"] = ocrResult.TextBlocks[1].Text;
                                    jsonObject["Name"] = ocrResult.TextBlocks[0].Text;
                                }
                                else if (i == 3)
                                {
                                    //jsonObject.Add("Place", ocrResult.TextBlocks[2].Text);
                                    //jsonObject.Add("Name", ocrResult.TextBlocks[0].Text + ocrResult.TextBlocks[1].Text);
                                    jsonObject["Place"] = ocrResult.TextBlocks[2].Text;
                                    jsonObject["Name"] = ocrResult.TextBlocks[0].Text + ocrResult.TextBlocks[1].Text;
                                }
                            }
                            break;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                // 使用默认桌面选项显示提示框，确保其在所有窗口的最上层
                MessageBox.Show(
                    "捕获到异常：" + ex.Message,
                    "异常",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error,
                    MessageBoxResult.OK,
                    MessageBoxOptions.DefaultDesktopOnly);
            }

            return jsonObject;
        }

        /// <summary>
        /// 获取字符串中第一次出现的连续数字
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string ExtractFirstNumber(string input)
        {
            // 定义正则表达式
            string pattern = @"\d[\d,]*\d|\d+"; // 匹配包含逗号和不包含逗号的数字序列
            Regex regex = new Regex(pattern);

            // 在输入文本中查找第一个匹配项
            Match match = regex.Match(input);

            if (match.Success)
            {
                return match.Value;
            }
            else
            {
                return "未找到数字";
            }
        }
    }
}
