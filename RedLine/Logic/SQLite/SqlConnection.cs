using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.IO;
using System.Text;

namespace RedLine.Logic.SQLite
{
  public class SqlConnection
  {
    private byte[] DataArray { get; }

    private ulong DataEncoding { get; }

    public string[] Fields { get; set; }

    public int RowLength
    {
      get
      {
        return this.SqlRows.Length;
      }
    }

    private ushort PageSize { get; }

    private DataEntry[] DataEntries { get; set; }

    private SQLiteRow[] SqlRows { get; set; }

    private byte[] SQLDataTypeSize { get; }

    public SqlConnection(string baseName)
    {
      this.SQLDataTypeSize = new byte[10]
      {
        (byte) 0,
        (byte) 1,
        (byte) 2,
        (byte) 3,
        (byte) 4,
        (byte) 6,
        (byte) 8,
        (byte) 8,
        (byte) 0,
        (byte) 0
      };
      if (!File.Exists(baseName))
        return;
      FileSystem.FileOpen(1, baseName, OpenMode.Binary, OpenAccess.Read, OpenShare.Shared, -1);
      string s = Strings.Space((int) FileSystem.LOF(1));
      FileSystem.FileGet(1, ref s, -1L, false);
      FileSystem.FileClose(1);
      this.DataArray = Encoding.Default.GetBytes(s);
      this.PageSize = (ushort) this.ToUInt64(16, 2);
      this.DataEncoding = this.ToUInt64(56, 4);
      if (Decimal.Compare(new Decimal(this.DataEncoding), Decimal.Zero) == 0)
        this.DataEncoding = 1UL;
      this.ReadDataEntries(100UL);
    }

    public string[] ParseTables()
    {
      string[] strArray = (string[]) null;
      int index1 = 0;
      int num = this.DataEntries.Length - 1;
      for (int index2 = 0; index2 <= num; ++index2)
      {
        if (this.DataEntries[index2].Type == "table")
        {
          strArray = (string[]) Utils.CopyArray((Array) strArray, (Array) new string[index1 + 1]);
          strArray[index1] = this.DataEntries[index2].Name;
          ++index1;
        }
      }
      return strArray;
    }

    public string ParseValue(int rowIndex, int fieldIndex)
    {
      if (rowIndex >= this.SqlRows.Length)
        return (string) null;
      if (fieldIndex >= this.SqlRows[rowIndex].RowData.Length)
        return (string) null;
      return this.SqlRows[rowIndex].RowData[fieldIndex];
    }

    public string ParseValue(int rowIndex, string fieldName)
    {
      int fieldIndex = -1;
      int num = this.Fields.Length - 1;
      for (int index = 0; index <= num; ++index)
      {
        if (this.Fields[index].ToLower().Trim().CompareTo(fieldName.ToLower().Trim()) == 0)
        {
          fieldIndex = index;
          break;
        }
      }
      if (fieldIndex == -1)
        return (string) null;
      return this.ParseValue(rowIndex, fieldIndex);
    }

    public bool ReadTable(string tableName)
    {
      int index1 = -1;
      int num1 = this.DataEntries.Length - 1;
      for (int index2 = 0; index2 <= num1; ++index2)
      {
        if (this.DataEntries[index2].Name.ToLower().CompareTo(tableName.ToLower()) == 0)
        {
          index1 = index2;
          break;
        }
      }
      if (index1 == -1)
        return false;
      string[] strArray = this.DataEntries[index1].SqlStatement.Substring(this.DataEntries[index1].SqlStatement.IndexOf("(") + 1).Split(',');
      int num2 = strArray.Length - 1;
      for (int index2 = 0; index2 <= num2; ++index2)
      {
        strArray[index2] = strArray[index2].TrimStart();
        int length = strArray[index2].IndexOf(" ");
        if (length > 0)
          strArray[index2] = strArray[index2].Substring(0, length);
        if (strArray[index2].IndexOf("UNIQUE") != 0)
        {
          this.Fields = (string[]) Utils.CopyArray((Array) this.Fields, (Array) new string[index2 + 1]);
          this.Fields[index2] = strArray[index2];
        }
        else
          break;
      }
      return this.ReadDataEntriesFromOffsets((ulong) (this.DataEntries[index1].RootNum - 1L) * (ulong) this.PageSize);
    }

    private ulong ToUInt64(int startIndex, int Size)
    {
      if (Size > 8 || Size == 0)
        return 0;
      ulong num = 0;
      for (int index = 0; index <= Size - 1; ++index)
        num = num << 8 | (ulong) this.DataArray[startIndex + index];
      return num;
    }

    private long CalcVertical(int startIndex, int endIndex)
    {
      ++endIndex;
      byte[] numArray = new byte[8];
      int num1 = endIndex - startIndex;
      bool flag = false;
      if (num1 == 0 | num1 > 9)
        return 0;
      switch (num1)
      {
        case 1:
          numArray[0] = (byte) ((uint) this.DataArray[startIndex] & (uint) sbyte.MaxValue);
          return BitConverter.ToInt64(numArray, 0);
        case 9:
          flag = true;
          break;
      }
      int num2 = 1;
      int num3 = 7;
      int index1 = 0;
      if (flag)
      {
        numArray[0] = this.DataArray[endIndex - 1];
        --endIndex;
        index1 = 1;
      }
      int num4 = startIndex;
      for (int index2 = endIndex - 1; index2 >= num4; index2 += -1)
      {
        if (index2 - 1 >= startIndex)
        {
          numArray[index1] = (byte) ((uint) (byte) ((uint) this.DataArray[index2] >> (num2 - 1 & 7)) & (uint) ((int) byte.MaxValue >> num2) | (uint) (byte) ((uint) this.DataArray[index2 - 1] << (num3 & 7)));
          ++num2;
          ++index1;
          --num3;
        }
        else if (!flag)
          numArray[index1] = (byte) ((uint) (byte) ((uint) this.DataArray[index2] >> (num2 - 1 & 7)) & (uint) ((int) byte.MaxValue >> num2));
      }
      return BitConverter.ToInt64(numArray, 0);
    }

    private int GetValues(int startIndex)
    {
      if (startIndex > this.DataArray.Length)
        return 0;
      int num = startIndex + 8;
      for (int index = startIndex; index <= num; ++index)
      {
        if (index > this.DataArray.Length - 1)
          return 0;
        if (((int) this.DataArray[index] & 128) != 128)
          return index;
      }
      return startIndex + 8;
    }

    public static bool ItIsOdd(long value)
    {
      return (value & 1L) == 1L;
    }

    private void ReadDataEntries(ulong Offset)
    {
      if (this.DataArray[(int) Offset] == (byte) 13)
      {
        ushort num1 = (this.ToUInt64((Offset.ForceTo<Decimal>() + new Decimal(3)).ForceTo<int>(), 2).ForceTo<Decimal>() - Decimal.One).ForceTo<ushort>();
        int num2 = 0;
        if (this.DataEntries != null)
        {
          num2 = this.DataEntries.Length;
          this.DataEntries = (DataEntry[]) Utils.CopyArray((Array) this.DataEntries, (Array) new DataEntry[this.DataEntries.Length + (int) num1 + 1]);
        }
        else
          this.DataEntries = new DataEntry[(int) num1 + 1];
        int num3 = (int) num1;
        for (int index1 = 0; index1 <= num3; ++index1)
        {
          ulong uint64 = this.ToUInt64((Offset.ForceTo<Decimal>() + new Decimal(8) + (index1 * 2).ForceTo<Decimal>()).ForceTo<int>(), 2);
          if (Decimal.Compare(Offset.ForceTo<Decimal>(), new Decimal(100)) != 0)
            uint64 += Offset;
          int values1 = this.GetValues(uint64.ForceTo<int>());
          this.CalcVertical(uint64.ForceTo<int>(), values1);
          int values2 = this.GetValues((uint64.ForceTo<Decimal>() + values1.ForceTo<Decimal>() - uint64.ForceTo<Decimal>() + Decimal.One).ForceTo<int>());
          this.DataEntries[num2 + index1].ID = this.CalcVertical((uint64.ForceTo<Decimal>() + values1.ForceTo<Decimal>() - uint64.ForceTo<Decimal>() + Decimal.One).ForceTo<int>(), values2);
          ulong @this = (uint64.ForceTo<Decimal>() + values2.ForceTo<Decimal>() - uint64.ForceTo<Decimal>() + Decimal.One).ForceTo<ulong>();
          int values3 = this.GetValues(@this.ForceTo<int>());
          int endIndex = values3;
          long num4 = this.CalcVertical(@this.ForceTo<int>(), values3);
          long[] numArray = new long[5];
          int index2 = 0;
          do
          {
            int startIndex = endIndex + 1;
            endIndex = this.GetValues(startIndex);
            numArray[index2] = this.CalcVertical(startIndex, endIndex);
            numArray[index2] = numArray[index2] <= 9L ? (long) this.SQLDataTypeSize[(int) numArray[index2]] : (!SqlConnection.ItIsOdd(numArray[index2]) ? (long) Math.Round((double) (numArray[index2] - 12L) / 2.0) : (long) Math.Round((double) (numArray[index2] - 13L) / 2.0));
            ++index2;
          }
          while (index2 <= 4);
          Encoding encoding = Encoding.Default;
          Decimal num5 = this.DataEncoding.ForceTo<Decimal>();
          Decimal num6 = Decimal.One;
          if (!num6.Equals(num5))
          {
            num6 = new Decimal(2);
            if (!num6.Equals(num5))
            {
              num6 = new Decimal(3);
              if (num6.Equals(num5))
                encoding = Encoding.BigEndianUnicode;
            }
            else
              encoding = Encoding.Unicode;
          }
          else
            encoding = Encoding.Default;
          this.DataEntries[num2 + index1].Type = encoding.GetString(this.DataArray, Convert.ToInt32(Decimal.Add(new Decimal(@this), new Decimal(num4))), (int) numArray[0]);
          this.DataEntries[num2 + index1].Name = encoding.GetString(this.DataArray, Convert.ToInt32(Decimal.Add(Decimal.Add(new Decimal(@this), new Decimal(num4)), new Decimal(numArray[0]))), (int) numArray[1]);
          this.DataEntries[num2 + index1].RootNum = (long) this.ToUInt64(Convert.ToInt32(Decimal.Add(Decimal.Add(Decimal.Add(Decimal.Add(new Decimal(@this), new Decimal(num4)), new Decimal(numArray[0])), new Decimal(numArray[1])), new Decimal(numArray[2]))), (int) numArray[3]);
          this.DataEntries[num2 + index1].SqlStatement = encoding.GetString(this.DataArray, Convert.ToInt32(Decimal.Add(Decimal.Add(Decimal.Add(Decimal.Add(Decimal.Add(new Decimal(@this), new Decimal(num4)), new Decimal(numArray[0])), new Decimal(numArray[1])), new Decimal(numArray[2])), new Decimal(numArray[3]))), (int) numArray[4]);
        }
      }
      else
      {
        if (this.DataArray[(int) Offset] != (byte) 5)
          return;
        int uint16 = (int) Convert.ToUInt16(Decimal.Subtract(new Decimal(this.ToUInt64(Convert.ToInt32(Decimal.Add(new Decimal(Offset), new Decimal(3))), 2)), Decimal.One));
        for (int index = 0; index <= uint16; ++index)
        {
          ushort uint64 = (ushort) this.ToUInt64(Convert.ToInt32(Decimal.Add(Decimal.Add(new Decimal(Offset), new Decimal(12)), new Decimal(index * 2))), 2);
          if (Decimal.Compare(new Decimal(Offset), new Decimal(100)) == 0)
            this.ReadDataEntries(Convert.ToUInt64(Decimal.Multiply(Decimal.Subtract(new Decimal(this.ToUInt64((int) uint64, 4)), Decimal.One), new Decimal((int) this.PageSize))));
          else
            this.ReadDataEntries(Convert.ToUInt64(Decimal.Multiply(Decimal.Subtract(new Decimal(this.ToUInt64((int) ((long) Offset + (long) uint64), 4)), Decimal.One), new Decimal((int) this.PageSize))));
        }
        this.ReadDataEntries(Convert.ToUInt64(Decimal.Multiply(Decimal.Subtract(new Decimal(this.ToUInt64(Convert.ToInt32(Decimal.Add(new Decimal(Offset), new Decimal(8))), 4)), Decimal.One), new Decimal((int) this.PageSize))));
      }
    }

    private bool ReadDataEntriesFromOffsets(ulong Offset)
    {
      if (this.DataArray[(int) Offset] == (byte) 13)
      {
        int int32 = Convert.ToInt32(Decimal.Subtract(new Decimal(this.ToUInt64(Convert.ToInt32(Decimal.Add(new Decimal(Offset), new Decimal(3))), 2)), Decimal.One));
        int num1 = 0;
        if (this.SqlRows != null)
        {
          num1 = this.SqlRows.Length;
          this.SqlRows = (SQLiteRow[]) Utils.CopyArray((Array) this.SqlRows, (Array) new SQLiteRow[this.SqlRows.Length + int32 + 1]);
        }
        else
          this.SqlRows = new SQLiteRow[int32 + 1];
        int num2 = int32;
        for (int index1 = 0; index1 <= num2; ++index1)
        {
          TypeSizes[] typeSizesArray = (TypeSizes[]) null;
          ulong uint64_1 = this.ToUInt64(Convert.ToInt32(Decimal.Add(Decimal.Add(new Decimal(Offset), new Decimal(8)), new Decimal(index1 * 2))), 2);
          if (Decimal.Compare(new Decimal(Offset), new Decimal(100)) != 0)
            uint64_1 += Offset;
          int values1 = this.GetValues((int) uint64_1);
          this.CalcVertical((int) uint64_1, values1);
          int values2 = this.GetValues(Convert.ToInt32(Decimal.Add(Decimal.Add(new Decimal(uint64_1), Decimal.Subtract(new Decimal(values1), new Decimal(uint64_1))), Decimal.One)));
          this.SqlRows[num1 + index1].ID = this.CalcVertical(Convert.ToInt32(Decimal.Add(Decimal.Add(new Decimal(uint64_1), Decimal.Subtract(new Decimal(values1), new Decimal(uint64_1))), Decimal.One)), values2);
          ulong uint64_2 = Convert.ToUInt64(Decimal.Add(Decimal.Add(new Decimal(uint64_1), Decimal.Subtract(new Decimal(values2), new Decimal(uint64_1))), Decimal.One));
          int values3 = this.GetValues((int) uint64_2);
          int endIndex = values3;
          long num3 = this.CalcVertical((int) uint64_2, values3);
          long num4 = Convert.ToInt64(Decimal.Add(Decimal.Subtract(new Decimal(uint64_2), new Decimal(values3)), Decimal.One));
          int index2 = 0;
          while (num4 < num3)
          {
            typeSizesArray = (TypeSizes[]) Utils.CopyArray((Array) typeSizesArray, (Array) new TypeSizes[index2 + 1]);
            int startIndex = endIndex + 1;
            endIndex = this.GetValues(startIndex);
            typeSizesArray[index2].Type = this.CalcVertical(startIndex, endIndex);
            typeSizesArray[index2].Size = typeSizesArray[index2].Type <= 9L ? (long) this.SQLDataTypeSize[(int) typeSizesArray[index2].Type] : (!SqlConnection.ItIsOdd(typeSizesArray[index2].Type) ? (long) Math.Round((double) (typeSizesArray[index2].Type - 12L) / 2.0) : (long) Math.Round((double) (typeSizesArray[index2].Type - 13L) / 2.0));
            num4 = num4 + (long) (endIndex - startIndex) + 1L;
            ++index2;
          }
          this.SqlRows[num1 + index1].RowData = new string[typeSizesArray.Length - 1 + 1];
          int num5 = 0;
          int num6 = typeSizesArray.Length - 1;
          for (int index3 = 0; index3 <= num6; ++index3)
          {
            if (typeSizesArray[index3].Type > 9L)
            {
              if (!SqlConnection.ItIsOdd(typeSizesArray[index3].Type))
              {
                if (Decimal.Compare(new Decimal(this.DataEncoding), Decimal.One) == 0)
                  this.SqlRows[num1 + index1].RowData[index3] = Encoding.Default.GetString(this.DataArray, Convert.ToInt32(Decimal.Add(Decimal.Add(new Decimal(uint64_2), new Decimal(num3)), new Decimal(num5))), (int) typeSizesArray[index3].Size);
                else if (Decimal.Compare(new Decimal(this.DataEncoding), new Decimal(2)) == 0)
                  this.SqlRows[num1 + index1].RowData[index3] = Encoding.Unicode.GetString(this.DataArray, Convert.ToInt32(Decimal.Add(Decimal.Add(new Decimal(uint64_2), new Decimal(num3)), new Decimal(num5))), (int) typeSizesArray[index3].Size);
                else if (Decimal.Compare(new Decimal(this.DataEncoding), new Decimal(3)) == 0)
                  this.SqlRows[num1 + index1].RowData[index3] = Encoding.BigEndianUnicode.GetString(this.DataArray, Convert.ToInt32(Decimal.Add(Decimal.Add(new Decimal(uint64_2), new Decimal(num3)), new Decimal(num5))), (int) typeSizesArray[index3].Size);
              }
              else
                this.SqlRows[num1 + index1].RowData[index3] = Encoding.Default.GetString(this.DataArray, Convert.ToInt32(Decimal.Add(Decimal.Add(new Decimal(uint64_2), new Decimal(num3)), new Decimal(num5))), (int) typeSizesArray[index3].Size);
            }
            else
              this.SqlRows[num1 + index1].RowData[index3] = Convert.ToString(this.ToUInt64(Convert.ToInt32(Decimal.Add(Decimal.Add(new Decimal(uint64_2), new Decimal(num3)), new Decimal(num5))), (int) typeSizesArray[index3].Size));
            num5 += (int) typeSizesArray[index3].Size;
          }
        }
      }
      else if (this.DataArray[(int) Offset] == (byte) 5)
      {
        int uint16 = (int) Convert.ToUInt16(Decimal.Subtract(new Decimal(this.ToUInt64(Convert.ToInt32(Decimal.Add(new Decimal(Offset), new Decimal(3))), 2)), Decimal.One));
        for (int index = 0; index <= uint16; ++index)
        {
          ushort uint64 = (ushort) this.ToUInt64(Convert.ToInt32(Decimal.Add(Decimal.Add(new Decimal(Offset), new Decimal(12)), new Decimal(index * 2))), 2);
          this.ReadDataEntriesFromOffsets(Convert.ToUInt64(Decimal.Multiply(Decimal.Subtract(new Decimal(this.ToUInt64((int) ((long) Offset + (long) uint64), 4)), Decimal.One), new Decimal((int) this.PageSize))));
        }
        this.ReadDataEntriesFromOffsets(Convert.ToUInt64(Decimal.Multiply(Decimal.Subtract(new Decimal(this.ToUInt64(Convert.ToInt32(Decimal.Add(new Decimal(Offset), new Decimal(8))), 4)), Decimal.One), new Decimal((int) this.PageSize))));
      }
      return true;
    }
  }
}
