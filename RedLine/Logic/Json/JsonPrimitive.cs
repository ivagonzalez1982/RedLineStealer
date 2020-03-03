using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace RedLine.Logic.Json
{
  public class JsonPrimitive : JsonValue
  {
    private static readonly byte[] true_bytes = Encoding.UTF8.GetBytes("true");
    private static readonly byte[] false_bytes = Encoding.UTF8.GetBytes("false");
    private object value;

    public JsonPrimitive(bool value)
    {
      this.value = (object) value;
    }

    public JsonPrimitive(byte value)
    {
      this.value = (object) value;
    }

    public JsonPrimitive(char value)
    {
      this.value = (object) value;
    }

    public JsonPrimitive(Decimal value)
    {
      this.value = (object) value;
    }

    public JsonPrimitive(double value)
    {
      this.value = (object) value;
    }

    public JsonPrimitive(float value)
    {
      this.value = (object) value;
    }

    public JsonPrimitive(int value)
    {
      this.value = (object) value;
    }

    public JsonPrimitive(long value)
    {
      this.value = (object) value;
    }

    public JsonPrimitive(sbyte value)
    {
      this.value = (object) value;
    }

    public JsonPrimitive(short value)
    {
      this.value = (object) value;
    }

    public JsonPrimitive(string value)
    {
      this.value = (object) value;
    }

    public JsonPrimitive(DateTime value)
    {
      this.value = (object) value;
    }

    public JsonPrimitive(uint value)
    {
      this.value = (object) value;
    }

    public JsonPrimitive(ulong value)
    {
      this.value = (object) value;
    }

    public JsonPrimitive(ushort value)
    {
      this.value = (object) value;
    }

    public JsonPrimitive(DateTimeOffset value)
    {
      this.value = (object) value;
    }

    public JsonPrimitive(Guid value)
    {
      this.value = (object) value;
    }

    public JsonPrimitive(TimeSpan value)
    {
      this.value = (object) value;
    }

    public JsonPrimitive(Uri value)
    {
      this.value = (object) value;
    }

    public JsonPrimitive(object value)
    {
      this.value = value;
    }

    public object Value
    {
      get
      {
        return this.value;
      }
    }

    public override JsonType JsonType
    {
      get
      {
        if (this.value == null)
          return JsonType.String;
        switch (Type.GetTypeCode(this.value.GetType()))
        {
          case TypeCode.Object:
          case TypeCode.Char:
          case TypeCode.DateTime:
          case TypeCode.String:
            return JsonType.String;
          case TypeCode.Boolean:
            return JsonType.Boolean;
          default:
            return JsonType.Number;
        }
      }
    }

    public override void Save(Stream stream, bool parsing)
    {
      switch (this.JsonType)
      {
        case JsonType.String:
          stream.WriteByte((byte) 34);
          byte[] bytes1 = Encoding.UTF8.GetBytes(this.EscapeString(this.value.ToString()));
          stream.Write(bytes1, 0, bytes1.Length);
          stream.WriteByte((byte) 34);
          break;
        case JsonType.Boolean:
          if ((bool) this.value)
          {
            stream.Write(JsonPrimitive.true_bytes, 0, 4);
            break;
          }
          stream.Write(JsonPrimitive.false_bytes, 0, 5);
          break;
        default:
          byte[] bytes2 = Encoding.UTF8.GetBytes(this.GetFormattedString());
          stream.Write(bytes2, 0, bytes2.Length);
          break;
      }
    }

    public string GetFormattedString()
    {
      switch (this.JsonType)
      {
        case JsonType.String:
          if (this.value is string || this.value == null)
          {
            string str = this.value as string;
            if (string.IsNullOrEmpty(str))
              return "null";
            return str.Trim('"');
          }
          if (this.value is char)
            return this.value.ToString();
          throw new NotImplementedException("GetFormattedString from value type " + (object) this.value.GetType());
        case JsonType.Number:
          string str1 = this.value is float || this.value is double ? ((IFormattable) this.value).ToString("R", (IFormatProvider) NumberFormatInfo.InvariantInfo) : ((IFormattable) this.value).ToString("G", (IFormatProvider) NumberFormatInfo.InvariantInfo);
          if (str1 == "NaN" || str1 == "Infinity" || str1 == "-Infinity")
            return "\"" + str1 + "\"";
          return str1;
        default:
          throw new InvalidOperationException();
      }
    }
  }
}
