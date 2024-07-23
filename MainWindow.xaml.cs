using FastHotKeyForWPF;
using game_tools.Helpers;
using System.Drawing;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static game_tools.Helpers.DataInitializer;
using Point = System.Drawing.Point;
using Brushes = System.Windows.Media.Brushes;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Emgu.CV.Ocl;


namespace game_tools.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private ScreenshotWindow screenshotWindow;
        private SQLiteDb sQLiteDb;
        public MainWindow(SQLiteDb sQLiteDb)
        {
            InitializeComponent();
            this.sQLiteDb = sQLiteDb;
            MainFrame.Navigate(new HomePage(this.sQLiteDb));
            //_sQLiteDb.Pizzas.Add(new Pizza { Name = "Margherita", Price = 7.99m });
            //_sQLiteDb.SaveChanges();
        }

        //-------------------------------------------------注册全局快捷键--------------------------------
        protected override void OnSourceInitialized(EventArgs e)
        {
            GlobalHotKey.Awake();//激活
            //选择识别
            GlobalHotKey.Add(ModelKeys.ALT, NormalKeys.F, SelectiveRecognition);
            //自动批量识别
            GlobalHotKey.Add(ModelKeys.ALT, NormalKeys.G, BatchOcrAsync);
            base.OnSourceInitialized(e);
        }

        /// <summary>
        /// 批量识别
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private async Task BatchOcrAsync()
        {
            SetWindow();
            if (MainFrame.Content is HomePage homePage)
            {
                await startTask(homePage);
            }
            //RestoreWindow();
        }

        protected override void OnClosed(EventArgs e)
        {
            GlobalHotKey.Destroy();//销毁
            base.OnClosed(e);
        }

        private void SetWindow()
        {
            // 获取屏幕工作区大小
            var screenWidth = SystemParameters.WorkArea.Width;
            var screenHeight = SystemParameters.WorkArea.Height;

            // 将窗口位置设置为左下角
            Left = 0;
            Top = screenHeight - Height;

            // 窗口始终保持在最顶层
            Topmost = true;
        }

        // 恢复窗口到正常状态的方法
        private void RestoreWindow()
        {
            Left = (SystemParameters.WorkArea.Width - Width) / 2;
            Top = (SystemParameters.WorkArea.Height - Height) / 2;
            Topmost = false;
        }

        /// <summary>
        /// 打开选择识别窗口
        /// </summary>
        private void SelectiveRecognition()
        {
            // 创建并显示新窗口
            if (screenshotWindow == null)
            {
                screenshotWindow = new game_tools.Views.ScreenshotWindow(this.sQLiteDb);
                screenshotWindow.Closed += (s, args) => screenshotWindow = null; // 当窗口关闭时，将 newWindow 设置为 null
                screenshotWindow.Show();
            }
            else
            {
                screenshotWindow.Activate(); // 如果窗口已经打开，激活它
            }
        }

        private void NavigateHome(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new HomePage(this.sQLiteDb));
        }

        private void NavigateProducts(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new HomePage(this.sQLiteDb));
        }

        private void NavigateServices(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new HomePage(this.sQLiteDb));
        }

        private void NavigateContact(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new HomePage(this.sQLiteDb));
        }

        private void NavigateSetting(object sender, RoutedEventArgs e)
        {
            //MainFrame.Navigate(new SettingPage());
        }

        public async Task startTask(HomePage homePage)
        {
            //初始化
            PaddleOCRHelp paddle = new PaddleOCRHelp(this.sQLiteDb);
            MouseHelp mouseHelp = new MouseHelp();
            //InitScreenPotions();

            //打开背包
            homePage.ResetBoxColor();
            await UserInputHelper.keybd_event_IAsync();

            //获取json数据
            JObject jsonObject = new JObject();
            jsonObject = GetScreenPotions();
            List<Detail> details = jsonObject["1080"].ToObject<List<Detail>>();
            var shuffledListWithIndexes = ShuffleListWithIndexes(details);

            var taskBatch =  this.sQLiteDb.Equipments.Select(p => p.TaskBatch).Max();

            var count_succeed = 0;
            var count_fail = 0;

            foreach (var item in shuffledListWithIndexes)
            {
                //移动鼠标
                await mouseHelp.SimulateMouseMovementAsync(new System.Windows.Point(item.Value.X, item.Value.Y));
                //截取屏幕指定区域
                Bitmap bitmap = OpenCVHelper.CaptureScreenAsync(item.Value.StratX, 0, item.Value.EndX - item.Value.StratX, 763);
                Point point = OpenCVHelper.FindImage(bitmap);
                bool isEmpty = point == Point.Empty;
                if (!isEmpty)
                {
                    count_succeed++;
                    //截取精确的识别区域
                    Bitmap bitmap1 = OpenCVHelper.CropBitmapBelowY(bitmap, point.Y);
                    //开始识别并保存
                    paddle.RecognizeAndSave(bitmap1, item.Index + 1,Convert.ToInt32(taskBatch)+1);
                    homePage.ChangeGridRectangleColor(Brushes.Green, item.Index + 1);
                    homePage.checkRow((item.Index + 1).ToString());
                }
                else
                {
                    count_fail++;
                    homePage.ChangeGridRectangleColor(Brushes.Red, item.Index + 1);
                }
            }

            // 使用默认桌面选项显示提示框，确保其在所有窗口的最上层
            MessageBox.Show(
                "任务执行结束！成功：" + count_succeed.ToString() + "失败：" + count_fail.ToString(),
                "提示",
                MessageBoxButton.OK,
                MessageBoxImage.Information,
                MessageBoxResult.OK,
                MessageBoxOptions.DefaultDesktopOnly);
        }

        private List<ItemWithIndex<T>> ShuffleListWithIndexes<T>(List<T> originalList)
        {
            Random random = new Random();
            List<ItemWithIndex<T>> shuffledListWithIndexes = new List<ItemWithIndex<T>>();

            // Create a list of ItemWithIndex objects
            for (int i = 0; i < originalList.Count; i++)
            {
                shuffledListWithIndexes.Add(new ItemWithIndex<T>(i, originalList[i]));
            }

            // Shuffle the list
            for (int i = shuffledListWithIndexes.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                ItemWithIndex<T> temp = shuffledListWithIndexes[i];
                shuffledListWithIndexes[i] = shuffledListWithIndexes[j];
                shuffledListWithIndexes[j] = temp;
            }

            return shuffledListWithIndexes;
        }
    }
    class ItemWithIndex<T>
    {
        public int Index { get; }
        public T Value { get; }

        public ItemWithIndex(int index, T value)
        {
            Index = index;
            Value = value;
        }
    }
}


