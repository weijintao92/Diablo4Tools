using game_tools.Helpers;
using game_tools.ViewModel;
using System.Data;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using Rectangle = System.Windows.Shapes.Rectangle;


namespace game_tools.Views
{
    /// <summary>
    /// HomePage.xaml 的交互逻辑
    /// </summary>
    public partial class HomePage : Page
    {
        public HomePageViewModel ViewModel { get; set; }
        private SQLiteDb sQLiteDb;

        private static readonly int Rows = 3;
        private static readonly int Columns = 11;
        private readonly Color[,] gridColors = new Color[Rows, Columns];

        public HomePage(SQLiteDb sQLiteDb)
        {
            InitializeComponent();
            this.sQLiteDb = sQLiteDb;
            ViewModel = new HomePageViewModel(sQLiteDb);
            DataContext = ViewModel;
            Loaded += MainWindow_Loaded; // 订阅窗口加载事件
        }

        /// <summary>
        /// 查找特定名称的子控件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <param name="childName"></param>
        /// <returns></returns>
        private T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            if (parent == null) return null;

            // 遍历 parent 的子元素，查找指定名称的控件
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typedChild && (typedChild as FrameworkElement)?.Name == childName)
                {
                    return typedChild;
                }

                // 递归查找子控件
                T result = FindChild<T>(child, childName);
                if (result != null)
                    return result;
            }

            return null;
        }


        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 假设你的DataGrid的名称是myDataGrid
            if (MyDataGrid.SelectedItem == null)
            {
                // 没有选中任何行
                return;
            }

            // 获取选中的行
            var selectedItem = MyDataGrid.SelectedItem;

            // 检查选中的行是否为空（可以根据你的数据对象类型进行具体的检查）
            if (selectedItem is DataRowView rowView)
            {
                return;
            }

            // 如果不是DataRowView类型的数据，根据你的具体数据类型进行检查
            // 示例：假设数据对象类型是MyDataObject
            var dataObject = selectedItem as Equipments;
            if (dataObject != null)
            {
                // 根据MyDataObject的属性进行检查
                if (string.IsNullOrEmpty(dataObject.ImagePath))
                {
                    return;
                }
                string imagePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dataObject.ImagePath);
                Uri uri = new Uri("file:///" + imagePath.Replace("\\", "/"));
                // 创建 BitmapImage 对象并设置 Source 属性
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = uri;
                bitmap.EndInit();

                // 设置 Image 控件的 Source 属性
                imageControl.Source = bitmap;
            }
        }

        public void ResetBoxColor()
        {
            for (int i = 1; i < 34; i++)
            {
                SetNamedRectanglesColor($"Rect_{i}", Brushes.Gray);
            }
        }

        private void SetNamedRectanglesColor(string rectangleName, SolidColorBrush color)
        {
            // 使用 FindName 方法找到指定名称的 Rectangle 控件
            Rectangle rect = FindChild<Rectangle>(this, rectangleName);

            if (rect != null)
            {
                // 设置背景颜色
                rect.Fill = color;
            }
        }

        private void InitGridBox()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    gridColors[i, j] = Colors.Gray;
                }
            }

            // 创建行和列
            for (int i = 0; i < Rows; i++)
            {
                ColorGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }

            // 添加列定义
            for (int j = 0; j < Columns; j++)
            {
                // 设置每列的宽度
                ColumnDefinition columnDefinition = new ColumnDefinition();

                // 示例：设置第一列宽度为Auto，其余列为*（填充剩余空间）

                columnDefinition.Width = new GridLength(1, GridUnitType.Star);

                ColorGrid.ColumnDefinitions.Add(columnDefinition);
            }

            // 创建每个格子
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    Rectangle rect = new Rectangle
                    {
                        Fill = new SolidColorBrush(gridColors[i, j]),
                        Stroke = Brushes.Black,
                        Height = 20,
                        StrokeThickness = 0.5,
                        Name = $"Rect_{i * Columns + j + 1}", // 设置 Rectangle 的 Name 属性
                        Tag = $"{i * Columns + j + 1}" // 设置 Tag 属性存储行列索引值
                    };
                    Grid.SetRow(rect, i);
                    Grid.SetColumn(rect, j);
                    ColorGrid.Children.Add(rect);

                    // 添加点击事件处理程序
                    rect.MouseLeftButtonDown += Rect_MouseLeftButtonDown;

                    // 创建 TextBlock 用于显示数字
                    TextBlock textBlock = new TextBlock
                    {
                        Text = $"{i * Columns + j + 1}", // 示例：在格子中显示数字，可以根据需要修改显示的内容
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        FontWeight = FontWeights.Bold,
                        Foreground = Brushes.White, // 文字颜色为白色，以便与背景对比
                        IsHitTestVisible = false
                    };

                    // 将 TextBlock 添加到格子中
                    Grid.SetRow(textBlock, i);
                    Grid.SetColumn(textBlock, j);
                    ColorGrid.Children.Add(textBlock);
                }
            }
        }

        private void Rect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 处理点击事件
            if (sender is Rectangle rect && rect.Tag is string tagIndex)
            {
                checkRow(tagIndex);
            }
        }

        public void checkRow(string tagIndex)
        {
            // 获取绑定的 MainViewModel 实例
            if (DataContext is HomePageViewModel viewModel)
            {
                // 访问 DataGrid 的数据源
                var dataSource = viewModel.Entities;

                // 在 DataGrid 中查找与 Tag 值匹配的行
                foreach (var item in dataSource)
                {
                    if (item.ItemIndex == short.Parse(tagIndex))
                    {
                        // 设置 DataGrid 的选中行
                        MyDataGrid.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        public void ChangeGridRectangleColor(SolidColorBrush color, int index)
        {
            // 在 Loaded 事件中查找第一个 Button 控件
            Rectangle rect = FindChild<Rectangle>(this, $"Rect_{index}");
            if (rect != null)
            {
                rect.Fill = color; // 修改颜色为红色
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitGridBox();
            if (MyDataGrid.Items.Count > 0)
            {
                // 选中第一行
                MyDataGrid.SelectedItem = MyDataGrid.Items[0];
                MyDataGrid.Focus(); // 焦点设置到 DataGrid，以便显示选中状态
            }
        }

    }
}
