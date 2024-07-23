using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace game_tools.Helpers
{
    /// <summary>
    /// 操作键鼠的工具类
    /// </summary>
    public static class UserInputHelper
    {
        private static readonly Random random = new Random(); // 声明一个 Random 对象，用于生成随机数

        // 引入 Windows API 函数
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetCursorPos(int x, int y);

        //[DllImport("user32.dll", SetLastError = true)]
        //private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        // 引入 user32.dll 以使用 SendInput 函数
        [DllImport("user32.dll")]
        private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        // 输入类型常量定义
        private const int INPUT_KEYBOARD = 1;
        private const byte VK_I = 0x49; // 'i' 键的虚拟键码
        private const byte VK_SPACE = 0x20; // 空格键
        private const uint KEYEVENTF_KEYUP = 0x02; // 按键抬起的标志

        // 输入结构体定义
        [StructLayout(LayoutKind.Sequential)]
        private struct INPUT
        {
            public int Type;
            public InputUnion U;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct InputUnion
        {
            [FieldOffset(0)]
            public KEYBDINPUT ki;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public UIntPtr dwExtraInfo;
        }

        /// <summary>
        /// 按键I,打开背包
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static async Task keybd_event_IAsync()
        {
            keybd_event(VK_I, 0, 0, UIntPtr.Zero);

            int delayMilliseconds = random.Next(100, 200); // 生成500到2000毫秒之间的随机延迟
            await Task.Delay(delayMilliseconds); // 随机延迟

            keybd_event(VK_I, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
            await Task.Delay(random.Next(2000, 3000)); // 随机延迟
        }

        /// <summary>
        /// 按键空格，标记装备
        /// </summary>
        public static async Task keybd_event_SpaceAsync()
        {
            // 按下空格键
            keybd_event(VK_SPACE, 0, 0, UIntPtr.Zero);

            int delayMilliseconds = random.Next(100, 200); // 生成500到2000毫秒之间的随机延迟
            await Task.Delay(delayMilliseconds); // 随机延迟

            // 释放空格键
            keybd_event(VK_SPACE, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
        }

        // 模拟空格键的按下和释放
        private static void SendInputSpaceKey()
        {
            INPUT[] inputs = new INPUT[]
            {
                new INPUT
                {
                    Type = INPUT_KEYBOARD,
                    U = new InputUnion
                    {
                        ki = new KEYBDINPUT
                        {
                            wVk = VK_SPACE,
                            dwFlags = 0
                        }
                    }
                },
                new INPUT
                {
                    Type = INPUT_KEYBOARD,
                    U = new InputUnion
                    {
                        ki = new KEYBDINPUT
                        {
                            wVk = VK_SPACE,
                            dwFlags = KEYEVENTF_KEYUP
                        }
                    }
                }
            };

            // 发送输入
            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
        }
    }
}
