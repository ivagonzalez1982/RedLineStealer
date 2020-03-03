using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace RedLine.Logic.Json
{
  public class JavaScriptReader
  {
    private int Line = 1;
    private readonly StringBuilder SBuilder;
    private readonly TextReader Reader;
    private int Column;
    private int Peek;
    private bool HasPeek;
    private bool Prev_Lf;

    public JavaScriptReader(TextReader reader)
    {
      TextReader textReader = reader;
      if (textReader == null)
        throw new ArgumentNullException(nameof (reader));
      this.Reader = textReader;
      this.SBuilder = new StringBuilder();
    }

    public object Read()
    {
      object obj = this.ReadCore();
      this.SkipSpaces();
      if (this.ReadChar() >= 0)
        throw this.JsonError(string.Format("extra characters in JSON input"));
      return obj;
    }

    private object ReadCore()
    {
      this.SkipSpaces();
      int num1 = this.PeekChar();
      if (num1 < 0)
        throw this.JsonError("Incomplete JSON input");
      switch (num1)
      {
        case 34:
          return (object) this.ReadStringLiteral();
        case 91:
          this.ReadChar();
          List<object> objectList = new List<object>();
          this.SkipSpaces();
          if (this.PeekChar() == 93)
          {
            this.ReadChar();
            return (object) objectList;
          }
          while (true)
          {
            object obj = this.ReadCore();
            objectList.Add(obj);
            this.SkipSpaces();
            if (this.PeekChar() == 44)
              this.ReadChar();
            else
              break;
          }
          if (this.ReadChar() != 93)
            throw this.JsonError("JSON array must end with ']'");
          return (object) objectList.ToArray();
        case 102:
          this.Expect("false");
          return (object) false;
        case 110:
          this.Expect("null");
          return (object) null;
        case 116:
          this.Expect("true");
          return (object) true;
        case 123:
          this.ReadChar();
          Dictionary<string, object> dictionary = new Dictionary<string, object>();
          this.SkipSpaces();
          if (this.PeekChar() == 125)
          {
            this.ReadChar();
            return (object) dictionary;
          }
label_12:
          this.SkipSpaces();
          if (this.PeekChar() == 125)
          {
            this.ReadChar();
          }
          else
          {
            string index = this.ReadStringLiteral();
            this.SkipSpaces();
            this.Expect(':');
            this.SkipSpaces();
            dictionary[index] = this.ReadCore();
            this.SkipSpaces();
            switch (this.ReadChar())
            {
              case 125:
                break;
              default:
                goto label_12;
            }
          }
          int num2 = 0;
          KeyValuePair<string, object>[] keyValuePairArray = new KeyValuePair<string, object>[dictionary.Count];
          foreach (KeyValuePair<string, object> keyValuePair in dictionary)
            keyValuePairArray[num2++] = keyValuePair;
          return (object) keyValuePairArray;
        default:
          if (48 <= num1 && num1 <= 57 || num1 == 45)
            return this.ReadNumericLiteral();
          throw this.JsonError(string.Format("Unexpected character '{0}'", (object) (char) num1));
      }
    }

    private int PeekChar()
    {
      if (!this.HasPeek)
      {
        this.Peek = this.Reader.Read();
        this.HasPeek = true;
      }
      return this.Peek;
    }

    private int ReadChar()
    {
      int num = this.HasPeek ? this.Peek : this.Reader.Read();
      this.HasPeek = false;
      if (this.Prev_Lf)
      {
        ++this.Line;
        this.Column = 0;
        this.Prev_Lf = false;
      }
      if (num == 10)
        this.Prev_Lf = true;
      ++this.Column;
      return num;
    }

    private void SkipSpaces()
    {
      while (true)
      {
        switch (this.PeekChar())
        {
          case 9:
          case 10:
          case 13:
          case 32:
            this.ReadChar();
            continue;
          default:
            goto label_2;
        }
      }
label_2:;
    }

    private object ReadNumericLiteral()
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (this.PeekChar() == 45)
        stringBuilder.Append((char) this.ReadChar());
      int num1 = 0;
      bool flag1 = this.PeekChar() == 48;
      while (true)
      {
        int num2 = this.PeekChar();
        if (num2 >= 48 && 57 >= num2)
        {
          stringBuilder.Append((char) this.ReadChar());
          if (!flag1 || num1 != 1)
            ++num1;
          else
            break;
        }
        else
          goto label_7;
      }
      throw this.JsonError("leading zeros are not allowed");
label_7:
      if (num1 == 0)
        throw this.JsonError("Invalid JSON numeric literal; no digit found");
      bool flag2 = false;
      int num3 = 0;
      if (this.PeekChar() == 46)
      {
        flag2 = true;
        stringBuilder.Append((char) this.ReadChar());
        if (this.PeekChar() < 0)
          throw this.JsonError("Invalid JSON numeric literal; extra dot");
        while (true)
        {
          int num2 = this.PeekChar();
          if (num2 >= 48 && 57 >= num2)
          {
            stringBuilder.Append((char) this.ReadChar());
            ++num3;
          }
          else
            break;
        }
        if (num3 == 0)
          throw this.JsonError("Invalid JSON numeric literal; extra dot");
      }
      switch (this.PeekChar())
      {
        case 69:
        case 101:
          stringBuilder.Append((char) this.ReadChar());
          if (this.PeekChar() < 0)
            throw new ArgumentException("Invalid JSON numeric literal; incomplete exponent");
          switch (this.PeekChar())
          {
            case 43:
              stringBuilder.Append((char) this.ReadChar());
              break;
            case 45:
              stringBuilder.Append((char) this.ReadChar());
              break;
          }
          if (this.PeekChar() < 0)
            throw this.JsonError("Invalid JSON numeric literal; incomplete exponent");
          while (true)
          {
            int num2 = this.PeekChar();
            if (num2 >= 48 && 57 >= num2)
              stringBuilder.Append((char) this.ReadChar());
            else
              break;
          }
        default:
          if (!flag2)
          {
            int result1;
            if (int.TryParse(stringBuilder.ToString(), NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result1))
              return (object) result1;
            long result2;
            if (long.TryParse(stringBuilder.ToString(), NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result2))
              return (object) result2;
            ulong result3;
            if (ulong.TryParse(stringBuilder.ToString(), NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result3))
              return (object) result3;
          }
          Decimal result;
          if (Decimal.TryParse(stringBuilder.ToString(), NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result) && result != Decimal.Zero)
            return (object) result;
          break;
      }
      return (object) double.Parse(stringBuilder.ToString(), NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture);
    }

    private string ReadStringLiteral()
    {
      if (this.PeekChar() != 34)
        throw this.JsonError("Invalid JSON string literal format");
      this.ReadChar();
      this.SBuilder.Length = 0;
      while (true)
      {
        int num1 = this.ReadChar();
        if (num1 >= 0)
        {
          switch (num1)
          {
            case 34:
              goto label_6;
            case 92:
              int num2 = this.ReadChar();
              if (num2 >= 0)
              {
                switch (num2)
                {
                  case 34:
                  case 47:
                  case 92:
                    this.SBuilder.Append((char) num2);
                    continue;
                  case 98:
                    this.SBuilder.Append('\b');
                    continue;
                  case 102:
                    this.SBuilder.Append('\f');
                    continue;
                  case 110:
                    this.SBuilder.Append('\n');
                    continue;
                  case 114:
                    this.SBuilder.Append('\r');
                    continue;
                  case 116:
                    this.SBuilder.Append('\t');
                    continue;
                  case 117:
                    ushort num3 = 0;
                    for (int index = 0; index < 4; ++index)
                    {
                      num3 <<= 4;
                      int num4;
                      if ((num4 = this.ReadChar()) < 0)
                        throw this.JsonError("Incomplete unicode character escape literal");
                      if (48 <= num4 && num4 <= 57)
                        num3 += (ushort) (num4 - 48);
                      if (65 <= num4 && num4 <= 70)
                        num3 += (ushort) (num4 - 65 + 10);
                      if (97 <= num4 && num4 <= 102)
                        num3 += (ushort) (num4 - 97 + 10);
                    }
                    this.SBuilder.Append((char) num3);
                    continue;
                  default:
                    goto label_29;
                }
              }
              else
                goto label_9;
            default:
              this.SBuilder.Append((char) num1);
              continue;
          }
        }
        else
          break;
      }
      throw this.JsonError("JSON string is not closed");
label_6:
      return this.SBuilder.ToString();
label_9:
      throw this.JsonError("Invalid JSON string literal; incomplete escape sequence");
label_29:
      throw this.JsonError("Invalid JSON string literal; unexpected escape character");
    }

    private void Expect(char expected)
    {
      int num;
      if ((num = this.ReadChar()) != (int) expected)
        throw this.JsonError(string.Format("Expected '{0}', got '{1}'", (object) expected, (object) (char) num));
    }

    private void Expect(string expected)
    {
      for (int index = 0; index < expected.Length; ++index)
      {
        if (this.ReadChar() != (int) expected[index])
          throw this.JsonError(string.Format("Expected '{0}', differed at {1}", (object) expected, (object) index));
      }
    }

    private Exception JsonError(string msg)
    {
      return (Exception) new ArgumentException(string.Format("{0}. At line {1}, column {2}", (object) msg, (object) this.Line, (object) this.Column));
    }
  }
}
