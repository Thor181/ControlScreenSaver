using System.Runtime.InteropServices;

namespace ControlScreenSaver
{
    public static class ScreenSaver
    {
        private enum SpecialHandles
        {
            HWND_DESKTOP = 0x0,
            HWND_BROADCAST = 0xFFFF
        }

        [DllImport("User32.dll")]
        private static extern int SendMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int PostMessage(IntPtr hWnd,
           int wMsg, int wParam, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr OpenDesktop(
           string hDesktop, int Flags, bool Inherit,
           uint DesiredAccess);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool CloseDesktop(
           IntPtr hDesktop);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool EnumDesktopWindows(
           IntPtr hDesktop, EnumDesktopWindowsProc callback,
           IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool IsWindowVisible(
           IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetForegroundWindow();

        // Callbacks
        private delegate bool EnumDesktopWindowsProc(
           IntPtr hDesktop, IntPtr lParam);

        private const uint WM_SYSCOMMAND = 0x112;
        private const uint SC_SCREENSAVE = 0xF140;
        private const uint DESKTOP_WRITEOBJECTS = 0x0080;
        private const uint DESKTOP_READOBJECTS = 0x0001;
        private const int WM_CLOSE = 16;

        public static void On()
        {
            SendMessage(new IntPtr((int)SpecialHandles.HWND_BROADCAST), WM_SYSCOMMAND, SC_SCREENSAVE, 0);
        }

        public static void Off()
        {
            IntPtr hDesktop = OpenDesktop("Screen-saver", 0,
               false, DESKTOP_READOBJECTS | DESKTOP_WRITEOBJECTS);
            if (hDesktop != IntPtr.Zero)
            {
                EnumDesktopWindows(hDesktop, new
                   EnumDesktopWindowsProc(KillScreenSaverFunc),
                   IntPtr.Zero);
                CloseDesktop(hDesktop);
            }
            else
            {
                PostMessage(GetForegroundWindow(), WM_CLOSE,
                   0, 0);
            }
        }

        private static bool KillScreenSaverFunc(IntPtr hWnd,
           IntPtr lParam)
        {
            if (IsWindowVisible(hWnd))
                PostMessage(hWnd, WM_CLOSE, 0, 0);
            return true;
        }
    }
}
