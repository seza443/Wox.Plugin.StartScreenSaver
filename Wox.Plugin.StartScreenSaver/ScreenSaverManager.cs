using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;


// from http://stackoverflow.com/a/1737808/2179668
namespace Wox.Plugin.StartScreenSaver
{
    public class ScreenSaverInfo
    {
        public string FileName { get; set; }
        public string Name { get; set; }
    }

    public class ScreenSaverManager
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll")]
        static extern bool FreeLibrary(IntPtr hLibrary);

        [DllImport("user32")]
        static extern int LoadString(IntPtr hInstance, int wID, [Out] StringBuilder lpBuffer, int nBufferMax);

        public static IEnumerable<ScreenSaverInfo> GetScreenSavers(String filter)
        {
            string currentSSPath = null;
            using (RegistryKey desktopKey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop"))
            {
                if (desktopKey != null)
                {
                    string screenSaverExe = desktopKey.GetValue("SCRNSAVE.EXE") as string;
                    if (!string.IsNullOrEmpty(screenSaverExe))
                    {
                        currentSSPath = Path.GetDirectoryName(screenSaverExe);
                    }
                }
            }

            HashSet<string> directories = new HashSet<string>();
            directories.Add(Environment.GetFolderPath(Environment.SpecialFolder.System));
            directories.Add(Environment.GetFolderPath(Environment.SpecialFolder.SystemX86));
            if (currentSSPath != null)
                directories.Add(currentSSPath);

            foreach (string dir in directories)
            {
                foreach (string file in Directory.EnumerateFiles(dir, "*" + filter + "*.scr", SearchOption.TopDirectoryOnly))
                {
                    yield return GetScreenSaverInfo(file);
                }
            }
        }

        public static ScreenSaverInfo GetScreenSaverInfo(string filename)
        {
            IntPtr hLibrary = IntPtr.Zero;
            try
            {
                hLibrary = LoadLibrary(filename);
                StringBuilder sb = new StringBuilder(1024);
                LoadString(hLibrary, 1, sb, sb.Capacity);
                return new ScreenSaverInfo
                {
                    FileName = filename,
                    Name = sb.ToString()
                };
            }
            finally
            {
                if (hLibrary != IntPtr.Zero)
                    FreeLibrary(hLibrary);
            }
        }
    }
}
