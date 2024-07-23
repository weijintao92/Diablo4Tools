using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

namespace game_tools.Helpers
{
    public  class MouseHelp
    {
        private readonly Random random = new Random(); // 声明一个 Random 对象，用于生成随机数
        public async Task SimulateMouseMovementAsync(Point targetPosition)
        {
            // 计算当前鼠标位置
            Point currentPosition = GetMousePosition();

            // 模拟平滑移动
            for (int i = 0; i <= 100; i++)
            {
                double deltaX = targetPosition.X - currentPosition.X;
                double deltaY = targetPosition.Y - currentPosition.Y;

                double interpolation = i / 100.0;
                double newX = currentPosition.X + (deltaX * interpolation);
                double newY = currentPosition.Y + (deltaY * interpolation);

                SetMousePosition((int)newX, (int)newY);

                int delayMilliseconds = random.Next(5, 10); 
                await Task.Delay(delayMilliseconds); // 随机延迟
            }

            // 最终确认位置
            SetMousePosition((int)targetPosition.X, (int)targetPosition.Y);
            await Task.Delay(random.Next(500, 1000)); // 随机延迟
        }

        private Point GetMousePosition()
        {
            POINT p;
            GetCursorPos(out p);
            return new Point(p.X, p.Y);
        }

        private void SetMousePosition(int x, int y)
        {
            SetCursorPos(x, y);
        }

        // 调用Windows API
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }
    }
}

