using game_tools.Helpers;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;


namespace game_tools.Views
{
    /// <summary>
    /// Interaction logic for ScreenshotWindow.xaml
    /// </summary>
    public partial class ScreenshotWindow : Window
    {
        //选择区域
        private bool isDrawing = false;
        private bool hasDrawn = false;
        private System.Windows.Point startPoint;
        private bool isSelecting = false;
        private bool isResizing;
        private Cursor currentCursor;

        //矩形框内部
        private bool isDragging = false;
        private System.Windows.Point clickPosition;
        private double originalLeft;
        private double originalTop;

        //选择框边缘小点
        private bool isDraggingDot = false;
        private Ellipse currentDot;
        private double originalWidth;
        private double originalHeight;
        private double originalLeftDot;
        private double originalTopDot;
        private readonly SQLiteDb sQLiteDb;


        public ScreenshotWindow(SQLiteDb sQLiteDb)
        {
            InitializeComponent();
            this.sQLiteDb = sQLiteDb;
            //this.Style = null;

            this.MouseLeftButtonDown += MainWindow_MouseLeftButtonDown;
            this.MouseMove += MainWindow_MouseMove;
            this.MouseLeftButtonUp += MainWindow_MouseLeftButtonUp;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close(); // 按下 Esc 键时关闭窗口
            }
        }

        //-------------------------------------------------Rectangle边缘小点--------------------------------
        private void Dot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDraggingDot = true;
            currentDot = sender as Ellipse; ;
            clickPosition = e.GetPosition(canvas);
            originalWidth = selectionRectangle.Width;
            originalHeight = selectionRectangle.Height;
            originalLeft = Canvas.GetLeft(selectionRectangle);
            originalTop = Canvas.GetTop(selectionRectangle);
            currentDot.CaptureMouse();
            grid.Visibility = Visibility.Hidden;
        }

        private void Dot_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDraggingDot && currentDot != null)
            {

                System.Windows.Point currentPosition = e.GetPosition(canvas);
                double offsetX = currentPosition.X - clickPosition.X;
                double offsetY = currentPosition.Y - clickPosition.Y;

                if (currentDot == topLeftDot)
                {

                    selectionRectangle.Width = originalWidth - offsetX;
                    selectionRectangle.Height = originalHeight - offsetY;
                    Canvas.SetLeft(selectionRectangle, originalLeft + offsetX);
                    Canvas.SetTop(selectionRectangle, originalTop + offsetY);
                    //更新边缘小点组件位置
                    updateEllipseLocation();
                }
                else if (currentDot == topRightDot)
                {

                    selectionRectangle.Width = originalWidth + offsetX;
                    selectionRectangle.Height = originalHeight - offsetY;
                    Canvas.SetTop(selectionRectangle, originalTop + offsetY);
                    //更新边缘小点组件位置
                    updateEllipseLocation();

                }
                else if (currentDot == bottomLeftDot)
                {
                    selectionRectangle.Width = originalWidth - offsetX;
                    selectionRectangle.Height = originalHeight + offsetY;
                    Canvas.SetLeft(selectionRectangle, originalLeft + offsetX);
                    //更新边缘小点组件位置
                    updateEllipseLocation();
                }
                else if (currentDot == bottomRightDot)
                {
                    //更新选择区域
                    selectionRectangle.Width = originalWidth + offsetX;
                    selectionRectangle.Height = originalHeight + offsetY;
                    //更新边缘小点组件位置
                    updateEllipseLocation();
                }
                else if (currentDot == topDot)
                {
                    selectionRectangle.Height = originalHeight - offsetY;
                    Canvas.SetTop(selectionRectangle, originalTop + offsetY);
                    //更新边缘小点组件位置
                    updateEllipseLocation();
                }
                else if (currentDot == rightDot)
                {
                    selectionRectangle.Width = originalWidth + offsetX;
                    //更新边缘小点组件位置
                    updateEllipseLocation();
                }
                else if (currentDot == bottomDot)
                {
                    selectionRectangle.Height = originalHeight + offsetY;
                    //更新边缘小点组件位置
                    updateEllipseLocation();
                }
                else if (currentDot == leftDot)
                {
                    Canvas.SetLeft(selectionRectangle, originalLeft + offsetX);
                    selectionRectangle.Width = originalWidth - offsetX;
                    //更新边缘小点组件位置
                    updateEllipseLocation();
                }
            }
        }

        private void Dot_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDraggingDot = false;
            if (currentDot != null)
            {
                currentDot.ReleaseMouseCapture();
                currentDot = null;
                //设置附件元素
                setAllElement();
            }
        }

        /// <summary>
        /// 更新边缘小点位置
        /// </summary>
        private void updateEllipseLocation()
        {
            double x = Canvas.GetLeft(selectionRectangle);
            double y = Canvas.GetTop(selectionRectangle);
            double width = selectionRectangle.Width;
            double height = selectionRectangle.Height;
            // 小点
            Canvas.SetLeft(topLeftDot, x - 5);
            Canvas.SetTop(topLeftDot, y - 5);
            topLeftDot.Visibility = Visibility.Visible;

            Canvas.SetLeft(topRightDot, x + width - 5);
            Canvas.SetTop(topRightDot, y - 5);
            topRightDot.Visibility = Visibility.Visible;


            Canvas.SetLeft(bottomLeftDot, x - 5);
            Canvas.SetTop(bottomLeftDot, y + height - 5);
            bottomLeftDot.Visibility = Visibility.Visible;


            Canvas.SetLeft(bottomRightDot, x + width - 5);
            Canvas.SetTop(bottomRightDot, y + height - 5);
            bottomRightDot.Visibility = Visibility.Visible;

            Canvas.SetLeft(topDot, x + width / 2 - 5);
            Canvas.SetTop(topDot, y - 5);
            topDot.Visibility = Visibility.Visible;

            Canvas.SetLeft(rightDot, x + width - 5);
            Canvas.SetTop(rightDot, y + height / 2 - 5);
            rightDot.Visibility = Visibility.Visible;

            Canvas.SetLeft(bottomDot, x + width / 2 - 5);
            Canvas.SetTop(bottomDot, y + height - 5);
            bottomDot.Visibility = Visibility.Visible;

            Canvas.SetLeft(leftDot, x - 5);
            Canvas.SetTop(leftDot, y + height / 2 - 5);
            leftDot.Visibility = Visibility.Visible;
        }

        //-------------------------------------------------Rectangle中间按下拖动--------------------------------

        private void Rectangle_MouseEnter(object sender, MouseEventArgs e)
        {
            // 当鼠标进入Rectangle区域时，将光标形状设置为Hand
            this.Cursor = Cursors.Hand;
        }

        private void Rectangle_MouseLeave(object sender, MouseEventArgs e)
        {
            // 当鼠标离开Rectangle区域时，将光标形状设置为Arrow
            this.Cursor = Cursors.Arrow;
        }

        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
            clickPosition = e.GetPosition(canvas);
            originalLeft = Canvas.GetLeft(selectionRectangle);
            originalTop = Canvas.GetTop(selectionRectangle);
            selectionRectangle.CaptureMouse(); // 捕获鼠标
            hiddenAllElement();
        }

        private void Rectangle_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                System.Windows.Point currentPosition = e.GetPosition(canvas);
                double offsetX = currentPosition.X - clickPosition.X;
                double offsetY = currentPosition.Y - clickPosition.Y;

                Canvas.SetLeft(selectionRectangle, originalLeft + offsetX);
                Canvas.SetTop(selectionRectangle, originalTop + offsetY);
            }
        

        }

        private void Rectangle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            selectionRectangle.ReleaseMouseCapture(); // 释放鼠标捕获
                                                      //设置附属元素
            setAllElement();
        }

        //-------------------------------------------------按下鼠标左键开始绘制矩形
        private void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (hasDrawn) return; // 如果已经画了一个矩形，不再允许画新的

            isDrawing = true;

            startPoint = e.GetPosition(this);
            selectionRectangle.Width = 0;
            selectionRectangle.Height = 0;
            selectionRectangle.Visibility = Visibility.Visible;
            Canvas.SetLeft(selectionRectangle, startPoint.X);
            Canvas.SetTop(selectionRectangle, startPoint.Y);
            isSelecting = true;
            grid.Visibility = Visibility.Collapsed;
        }

        private void MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDrawing) return;
            if (isSelecting)
            {
                System.Windows.Point currentPoint = e.GetPosition(this);
                double x = Math.Min(startPoint.X, currentPoint.X);
                double y = Math.Min(startPoint.Y, currentPoint.Y);
                double width = Math.Abs(startPoint.X - currentPoint.X);
                double height = Math.Abs(startPoint.Y - currentPoint.Y);

                Canvas.SetLeft(selectionRectangle, x);
                Canvas.SetTop(selectionRectangle, y);
                selectionRectangle.Width = width;
                selectionRectangle.Height = height;
            }
        }

        private void MainWindow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!isDrawing) return;
            isDrawing = false;
            hasDrawn = true; // 标记已经画了一个矩形

            double x = Canvas.GetLeft(selectionRectangle);
            double y = Canvas.GetTop(selectionRectangle);
            double width = selectionRectangle.Width;
            double height = selectionRectangle.Height;

            if (isSelecting && width > 50 && height > 50)
            {
                isSelecting = false;
                //设置菜单栏
                setAllElement();
            }
        }

        /// <summary>
        /// 设置附属元素
        /// </summary>
        private void setAllElement()
        {
            double x = Canvas.GetLeft(selectionRectangle);
            double y = Canvas.GetTop(selectionRectangle);
            double width = selectionRectangle.Width;
            double height = selectionRectangle.Height;

            //菜单栏
            Canvas.SetLeft(grid, x); // 设置按钮左边距离画布左边的距离为50
            Canvas.SetTop(grid, y + height + 2); // 设置按钮顶部距离画布顶部的距离为50
            grid.Visibility = Visibility.Visible;

            // 小点
            Canvas.SetLeft(topLeftDot, x - 5);
            Canvas.SetTop(topLeftDot, y - 5);
            topLeftDot.Visibility = Visibility.Visible;

            Canvas.SetLeft(topRightDot, x + width - 5);
            Canvas.SetTop(topRightDot, y - 5);
            topRightDot.Visibility = Visibility.Visible;


            Canvas.SetLeft(bottomLeftDot, x - 5);
            Canvas.SetTop(bottomLeftDot, y + height - 5);
            bottomLeftDot.Visibility = Visibility.Visible;


            Canvas.SetLeft(bottomRightDot, x + width - 5);
            Canvas.SetTop(bottomRightDot, y + height - 5);
            bottomRightDot.Visibility = Visibility.Visible;

            Canvas.SetLeft(topDot, x + width / 2 - 5);
            Canvas.SetTop(topDot, y - 5);
            topDot.Visibility = Visibility.Visible;

            Canvas.SetLeft(rightDot, x + width - 5);
            Canvas.SetTop(rightDot, y + height / 2 - 5);
            rightDot.Visibility = Visibility.Visible;

            Canvas.SetLeft(bottomDot, x + width / 2 - 5);
            Canvas.SetTop(bottomDot, y + height - 5);
            bottomDot.Visibility = Visibility.Visible;

            Canvas.SetLeft(leftDot, x - 5);
            Canvas.SetTop(leftDot, y + height / 2 - 5);
            leftDot.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 隐藏元素
        /// </summary>
        private void hiddenAllElement()
        {
            grid.Visibility = Visibility.Hidden;

            topLeftDot.Visibility = Visibility.Hidden;
            topRightDot.Visibility = Visibility.Hidden;
            bottomLeftDot.Visibility = Visibility.Hidden;
            bottomRightDot.Visibility = Visibility.Hidden;
            topDot.Visibility = Visibility.Hidden;
            rightDot.Visibility = Visibility.Hidden;
            bottomDot.Visibility = Visibility.Hidden;
            leftDot.Visibility = Visibility.Hidden;
        }


        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnAssess_Click(object sender, RoutedEventArgs e)
        {
            //保存选中区域
            //saveSelectedArea(selectionRectangle);
            SelectRegionAndRecognize();

        }

        /// <summary>
        /// canvas选定区域识别
        /// </summary>
        private  void SelectRegionAndRecognize()
        {
            if (selectionRectangle.Visibility == Visibility.Visible)
            {
                // 获取相对于 Canvas 的坐标
                double x = Canvas.GetLeft(selectionRectangle);
                double y = Canvas.GetTop(selectionRectangle);
                int width = (int)Math.Round(selectionRectangle.Width);
                int height = (int)Math.Round(selectionRectangle.Height);
                
                System.Windows.Point canvasPoint = new System.Windows.Point(x, y);
                // 将 Canvas 坐标转换为屏幕坐标
                System.Windows.Point screenPoint = canvas.PointToScreen(canvasPoint);

                Bitmap bitmap =  OpenCVHelper.CaptureScreenAsync((int)Math.Round(screenPoint.X), (int)Math.Round(screenPoint.Y), width, height);

                PaddleOCRHelp paddle = new PaddleOCRHelp(this.sQLiteDb);
                paddle.RecognizeAndSave(bitmap);
                // 弹出提示框
                MessageBox.Show($"识别成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}