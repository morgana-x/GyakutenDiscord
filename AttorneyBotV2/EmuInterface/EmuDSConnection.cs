using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AttorneyBotV2.EmuInterface
{
    internal class EmuDSConnection
    {
        [DllImport("user32.dll")]
        static extern bool PostMessage(nint hWnd, uint Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        static extern bool SendMessage(nint hWnd, uint Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        static extern bool SetFocus(nint hWnd);
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(nint hWnd);
        [DllImport("user32.dll")]
        static extern nint GetForegroundWindow();
        [DllImport("user32.dll")]
        static extern bool AttachThreadInput(nint idAttach, nint idAttachTo, bool fAtatch);

        const uint WM_KEYDOWN = 0x0100;
        const uint WM_KEYUP   = 0x0101;
        const uint WM_KILLFOCUS = 0x0008;
        const uint WM_SETFOCUS = 0x0007;
        Process proc;
        public bool Connected { get { return proc != null; } }
        public EmuDSConnection(string procName = "melonDS")
        {
            Process[] processes = Process.GetProcessesByName(procName);
            if (processes.Length > 0)
                proc = processes[0];
        }
        public async void SendKey(int key)
        {
            if (proc == null) return;
            var lastForegroundWindow = GetForegroundWindow();
            SetFocus(proc.MainWindowHandle);
           // SetForegroundWindow(proc.MainWindowHandle);
            await Task.Delay(60);
          //  AttachThreadInput(Process.GetCurrentProcess().MainWindowHandle, proc.MainWindowHandle, true);
            Console.WriteLine($"Sending {key} to emulator");
            PostMessage(proc.MainWindowHandle, WM_KEYDOWN, key, 0);
            await Task.Delay(100);
            Console.WriteLine($"Sending  keyup {key} to emulator");
            PostMessage(proc.MainWindowHandle, WM_KEYUP, key, 0);
            await Task.Delay(60);
           // SetForegroundWindow(lastForegroundWindow);
            SetFocus(lastForegroundWindow);
            // PostMessage(proc.MainWindowHandle, WM_KEYUP, key, 0);
            // Task.Delay(1000);
            // Console.WriteLine($"Sending  keyup {key} to emulator");
            // PostMessage(proc.MainWindowHandle, WM_KEYUP, key, 0);
            //Task.Delay(50);
            //PostMessage((int)proc.MainWindowHandle, WM_KILLFOCUS, (int)Process.GetCurrentProcess().Handle, 0);
        }
    }
}
