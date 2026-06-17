using System.Runtime.InteropServices;

namespace OtcDataService.Native;

internal static class Win32MessageBox
{
    private const uint MbIconInformation = 0x00000040;

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

    public static void ShowInfo(string text, string caption = AppInfo.Name)
    {
        MessageBox(IntPtr.Zero, text, caption, MbIconInformation);
    }
}
