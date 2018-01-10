using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Clicker
{
    public partial class Form1 : Form
    {
        public const int MouseeventfLeftdown = 0x02;
        public const int MouseeventfLeftup = 0x04;
        public const int MouseeventfRightdown = 0x08;
        public const int MouseeventfRightup = 0x10;

        public Form1()
        {
            InitializeComponent();
            keybd_event(0x90, 0x45, 0x1, (UIntPtr) 0);
            var hook = new Hook(0x90); //NumLock

            hook.KeyPressed += _hook_KeyPressed;
            hook.SetHook();
        }

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        private void _hook_KeyPressed(object sender, KeyPressEventArgs e)
        {
            timer1.Enabled = false;
            timer2.Enabled = false;
            timer3.Enabled = false;
            timer4.Enabled = false;
            timer5.Enabled = false;
            Visible = Visible;
            Activate();
        }

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public void MouseLeftClick(int a, int b)
        {
            Cursor.Position = new Point(a, b);
            mouse_event(MouseeventfLeftdown, 0, 0, 0, 0);
            mouse_event(MouseeventfLeftup, 0, 0, 0, 0);
        }

        public void MouseRightClick(int a, int b)
        {
            Cursor.Position = new Point(a, b);
            mouse_event(MouseeventfRightdown, 0, 0, 0, 0);
            mouse_event(MouseeventfRightup, 0, 0, 0, 0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer4.Enabled = true;
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            SendKeys.Send("{TAB}");
            timer2.Enabled = true;
            timer1.Enabled = false;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            SendKeys.Send("l");
            timer3.Enabled = true;
            timer2.Enabled = false;
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            MouseLeftClick(958, 514);
            timer1.Enabled = true;
            timer3.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void timer4_Tick_1(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            timer2.Enabled = true;
            timer3.Enabled = true;
            timer4.Enabled = false;
            if (checkBox1.Checked)
                timer5.Enabled = true;
            else
                timer5.Enabled = false;
        }

        private void timer5_Tick_1(object sender, EventArgs e)
        {
            SendKeys.Send("+3");
        }

        public class Hook : IDisposable
        {
            private readonly int _key;
            private readonly KeyboardHookProc _proc;
            private IntPtr _hHook = IntPtr.Zero;

            public Hook(int keyCode)
            {
                _key = keyCode;
                _proc = HookProc;
            }

            public void Dispose()
            {
                UnHook();
            }

            public event KeyPressEventHandler KeyPressed;

            public void SetHook()
            {
                var hInstance = LoadLibrary("User32");
                _hHook = SetWindowsHookEx(WhKeyboardLl, _proc, hInstance, 0);
            }

            public void UnHook()
            {
                UnhookWindowsHookEx(_hHook);
            }

            private IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam)
            {
                if (code >= 0 && wParam == (IntPtr) WhKeydown && Marshal.ReadInt32(lParam) == _key)
                    if (KeyPressed != null)
                        KeyPressed(this, new KeyPressEventArgs(Convert.ToChar(code)));

                return CallNextHookEx(_hHook, code, (int) wParam, lParam);
            }

            private delegate IntPtr KeyboardHookProc(int code, IntPtr wParam, IntPtr lParam);

            #region Declare WinAPI functions

            [DllImport("kernel32.dll")]
            private static extern IntPtr LoadLibrary(string lpFileName);

            [DllImport("user32.dll")]
            private static extern IntPtr SetWindowsHookEx(int idHook, KeyboardHookProc callback, IntPtr hInstance,
                uint threadId);

            [DllImport("user32.dll")]
            private static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, int wParam, IntPtr lParam);

            [DllImport("user32.dll")]
            private static extern bool UnhookWindowsHookEx(IntPtr hInstance);

            #endregion

            #region Constants

            private const int WhKeyboardLl = 13;
            private const int WhKeydown = 0x0100;

            #endregion
        }
    }
}