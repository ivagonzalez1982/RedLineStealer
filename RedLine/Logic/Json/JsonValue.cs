using RedLine.Logic.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

namespace RedLine.Logic.Json
{
  public abstract class JsonValue : IEnumerable
  {
    public static JsonValue Load(Stream stream)
    {
      if (stream == null)
        throw new ArgumentNullException(nameof (stream));
      return JsonValue.Load((TextReader) new StreamReader(stream, true));
    }

    public static JsonValue Load(TextReader textReader)
    {
      if (textReader == null)
        throw new ArgumentNullException(nameof (textReader));
      return JsonValue.ToJsonValue<object>(new JavaScriptReader(textReader).Read());
    }

    private static IEnumerable<KeyValuePair<string, JsonValue>> ToJsonPairEnumerable(
      IEnumerable<KeyValuePair<string, object>> kvpc)
    {
      foreach (KeyValuePair<string, object> keyValuePair in kvpc)
        yield return new KeyValuePair<string, JsonValue>(keyValuePair.Key, JsonValue.ToJsonValue<object>(keyValuePair.Value));
    }

    private static IEnumerable<JsonValue> ToJsonValueEnumerable(IEnumerable arr)
    {
      foreach (object ret in arr)
        yield return JsonValue.ToJsonValue<object>(ret);
    }

    public static JsonValue ToJsonValue<T>(T ret)
    {
      if ((object) ret == null)
        return (JsonValue) null;
      T obj1;
      if ((object) (obj1 = ret) is bool)
        return (JsonValue) new JsonPrimitive((bool) (object) obj1);
      T obj2;
      if ((object) (obj2 = ret) is byte)
        return (JsonValue) new JsonPrimitive((byte) (object) obj2);
      T obj3;
      if ((object) (obj3 = ret) is char)
        return (JsonValue) new JsonPrimitive((char) (object) obj3);
      if (ret is Decimal num)
        return (JsonValue) new JsonPrimitive(num);
      if (ret is double num)
        return (JsonValue) new JsonPrimitive(num);
      T obj4;
      if ((object) (obj4 = ret) is float)
        return (JsonValue) new JsonPrimitive((float) (object) obj4);
      if (ret is int num)
        return (JsonValue) new JsonPrimitive(num);
      if (ret is long num)
        return (JsonValue) new JsonPrimitive(num);
      T obj5;
      if ((object) (obj5 = ret) is sbyte)
        return (JsonValue) new JsonPrimitive((sbyte) (object) obj5);
      T obj6;
      if ((object) (obj6 = ret) is short)
        return (JsonValue) new JsonPrimitive((short) (object) obj6);
      if (ret is string str)
        return (JsonValue) new JsonPrimitive(str);
      T obj7;
      if ((object) (obj7 = ret) is uint)
        return (JsonValue) new JsonPrimitive((uint) (object) obj7);
      T obj8;
      if ((object) (obj8 = ret) is ulong)
        return (JsonValue) new JsonPrimitive((ulong) (object) obj8);
      T obj9;
      if ((object) (obj9 = ret) is ushort)
        return (JsonValue) new JsonPrimitive((ushort) (object) obj9);
      if (ret is DateTime dateTime)
        return (JsonValue) new JsonPrimitive(dateTime);
      if (ret is DateTimeOffset dateTimeOffset)
        return (JsonValue) new JsonPrimitive(dateTimeOffset);
      if (ret is Guid guid)
        return (JsonValue) new JsonPrimitive(guid);
      if (ret is TimeSpan timeSpan)
        return (JsonValue) new JsonPrimitive(timeSpan);
      if (ret is Uri uri)
        return (JsonValue) new JsonPrimitive(uri);
      IEnumerable<KeyValuePair<string, object>> kvpc = (object) ret as IEnumerable<KeyValuePair<string, object>>;
      if (kvpc != null)
        return (JsonValue) new JsonObject(JsonValue.ToJsonPairEnumerable(kvpc));
      IEnumerable arr = (object) ret as IEnumerable;
      if (arr != null)
        return (JsonValue) new JsonArray(JsonValue.ToJsonValueEnumerable(arr));
      if (!((object) ret is IEnumerable))
      {
        PropertyInfo[] properties = ret.GetType().GetProperties();
        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        foreach (PropertyInfo propertyInfo in properties)
          dictionary.Add(propertyInfo.Name, propertyInfo.GetValue((object) ret, (object[]) null).IsNull<object>((object) "null"));
        if (dictionary.Count > 0)
          return (JsonValue) new JsonObject(JsonValue.ToJsonPairEnumerable((IEnumerable<KeyValuePair<string, object>>) dictionary));
      }
      throw new NotSupportedException(string.Format("Unexpected parser return type: {0}", (object) ret.GetType()));
    }

    public static JsonValue Parse(string jsonString)
    {
      if (jsonString == null)
        throw new ArgumentNullException(nameof (jsonString));
      return JsonValue.Load((TextReader) new StringReader(jsonString));
    }

    public virtual int Count
    {
      get
      {
        throw new InvalidOperationException();
      }
    }

    public abstract JsonType JsonType { get; }

    public virtual JsonValue this[int index]
    {
      get
      {
        throw new InvalidOperationException();
      }
      set
      {
        throw new InvalidOperationException();
      }
    }

    public virtual JsonValue this[string key]
    {
      get
      {
        throw new InvalidOperationException();
      }
      set
      {
        throw new InvalidOperationException();
      }
    }

    public virtual bool ContainsKey(string key)
    {
      throw new InvalidOperationException();
    }

    public virtual void Save(Stream stream, bool parsing)
    {
      if (stream == null)
        throw new ArgumentNullException(nameof (stream));
      this.Save((TextWriter) new StreamWriter(stream), parsing);
    }

    public virtual void Save(TextWriter textWriter, bool parsing)
    {
      if (textWriter == null)
        throw new ArgumentNullException(nameof (textWriter));
      this.Savepublic(textWriter, parsing);
    }

    private void Savepublic(TextWriter w, bool saving)
    {
      switch (this.JsonType)
      {
        case JsonType.String:
          if (saving)
            w.Write('"');
          w.Write(this.EscapeString(((JsonPrimitive) this).GetFormattedString()));
          if (!saving)
            break;
          w.Write('"');
          break;
        case JsonType.Object:
          w.Write('{');
          bool flag1 = false;
          foreach (KeyValuePair<string, JsonValue> keyValuePair in (JsonObject) this)
          {
            if (flag1)
              w.Write(", ");
            w.Write('"');
            w.Write(this.EscapeString(keyValuePair.Key));
            w.Write("\": ");
            if (keyValuePair.Value == null)
              w.Write("null");
            else
              keyValuePair.Value.Savepublic(w, saving);
            flag1 = true;
          }
          w.Write('}');
          break;
        case JsonType.Array:
          w.Write('[');
          bool flag2 = false;
          foreach (JsonValue jsonValue in (IEnumerable<JsonValue>) this)
          {
            if (flag2)
              w.Write(", ");
            if (jsonValue != null)
              jsonValue.Savepublic(w, saving);
            else
              w.Write("null");
            flag2 = true;
          }
          w.Write(']');
          break;
        case JsonType.Boolean:
          w.Write((bool) this ? "true" : "false");
          break;
        default:
          w.Write(((JsonPrimitive) this).GetFormattedString());
          break;
      }
    }

    public string ToString(bool saving = true)
    {
      StringWriter stringWriter = new StringWriter();
      this.Save((TextWriter) stringWriter, saving);
      return stringWriter.ToString();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      throw new InvalidOperationException();
    }

    private bool NeedEscape(string src, int i)
    {
      char ch = src[i];
      if (ch < ' ' || ch == '"' || ch == '\\' || ch >= '\xD800' && ch <= '\xDBFF' && (i == src.Length - 1 || src[i + 1] < '\xDC00' || src[i + 1] > '\xDFFF') || (ch >= '\xDC00' && ch <= '\xDFFF' && (i == 0 || src[i - 1] < '\xD800' || src[i - 1] > '\xDBFF') || (ch == '\x2028' || ch == '\x2029')))
        return true;
      if (ch == '/' && i > 0)
        return src[i - 1] == '<';
      return false;
    }

    public string EscapeString(string src)
    {
      if (src == null)
        return (string) null;
      for (int index = 0; index < src.Length; ++index)
      {
        if (this.NeedEscape(src, index))
        {
          StringBuilder sb = new StringBuilder();
          if (index > 0)
            sb.Append(src, 0, index);
          return this.DoEscapeString(sb, src, index);
        }
      }
      return src;
    }

    private string DoEscapeString(StringBuilder sb, string src, int cur)
    {
      int startIndex = cur;
      for (int i = cur; i < src.Length; ++i)
      {
        if (this.NeedEscape(src, i))
        {
          sb.Append(src, startIndex, i - startIndex);
          switch (src[i])
          {
            case '\b':
              sb.Append("\\b");
              break;
            case '\t':
              sb.Append("\\t");
              break;
            case '\n':
              sb.Append("\\n");
              break;
            case '\f':
              sb.Append("\\f");
              break;
            case '\r':
              sb.Append("\\r");
              break;
            case '"':
              sb.Append("\\\"");
              break;
            case '/':
              sb.Append("\\/");
              break;
            case '\\':
              sb.Append("\\\\");
              break;
            default:
              sb.Append("\\u");
              sb.Append(((int) src[i]).ToString("x04"));
              break;
          }
          startIndex = i + 1;
        }
      }
      sb.Append(src, startIndex, src.Length - startIndex);
      return sb.ToString();
    }

    public static implicit operator JsonValue(bool value)
    {
      return (JsonValue) new JsonPrimitive(value);
    }

    public static implicit operator JsonValue(byte value)
    {
      return (JsonValue) new JsonPrimitive(value);
    }

    public static implicit operator JsonValue(char value)
    {
      return (JsonValue) new JsonPrimitive(value);
    }

    public static implicit operator JsonValue(Decimal value)
    {
      return (JsonValue) new JsonPrimitive(value);
    }

    public static implicit operator JsonValue(double value)
    {
      return (JsonValue) new JsonPrimitive(value);
    }

    public static implicit operator JsonValue(float value)
    {
      return (JsonValue) new JsonPrimitive(value);
    }

    public static implicit operator JsonValue(int value)
    {
      return (JsonValue) new JsonPrimitive(value);
    }

    public static implicit operator JsonValue(long value)
    {
      return (JsonValue) new JsonPrimitive(value);
    }

    public static implicit operator JsonValue(sbyte value)
    {
      return (JsonValue) new JsonPrimitive(value);
    }

    public static implicit operator JsonValue(short value)
    {
      return (JsonValue) new JsonPrimitive(value);
    }

    public static implicit operator JsonValue(string value)
    {
      return (JsonValue) new JsonPrimitive(value);
    }

    public static implicit operator JsonValue(uint value)
    {
      return (JsonValue) new JsonPrimitive(value);
    }

    public static implicit operator JsonValue(ulong value)
    {
      return (JsonValue) new JsonPrimitive(value);
    }

    public static implicit operator JsonValue(ushort value)
    {
      return (JsonValue) new JsonPrimitive(value);
    }

    public static implicit operator JsonValue(DateTime value)
    {
      return (JsonValue) new JsonPrimitive(value);
    }

    public static implicit operator JsonValue(DateTimeOffset value)
    {
      return (JsonValue) new JsonPrimitive(value);
    }

    public static implicit operator JsonValue(Guid value)
    {
      return (JsonValue) new JsonPrimitive(value);
    }

    public static implicit operator JsonValue(TimeSpan value)
    {
      return (JsonValue) new JsonPrimitive(value);
    }

    public static implicit operator JsonValue(Uri value)
    {
      return (JsonValue) new JsonPrimitive(value);
    }

    public static implicit operator bool(JsonValue value)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      return Convert.ToBoolean(((JsonPrimitive) value).Value, (IFormatProvider) NumberFormatInfo.InvariantInfo);
    }

    public static implicit operator byte(JsonValue value)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      return Convert.ToByte(((JsonPrimitive) value).Value, (IFormatProvider) NumberFormatInfo.InvariantInfo);
    }

    public static implicit operator char(JsonValue value)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      return Convert.ToChar(((JsonPrimitive) value).Value, (IFormatProvider) NumberFormatInfo.InvariantInfo);
    }

    public static implicit operator Decimal(JsonValue value)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      return Convert.ToDecimal(((JsonPrimitive) value).Value, (IFormatProvider) NumberFormatInfo.InvariantInfo);
    }

    public static implicit operator double(JsonValue value)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      return Convert.ToDouble(((JsonPrimitive) value).Value, (IFormatProvider) NumberFormatInfo.InvariantInfo);
    }

    public static implicit operator float(JsonValue value)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      return Convert.ToSingle(((JsonPrimitive) value).Value, (IFormatProvider) NumberFormatInfo.InvariantInfo);
    }

    public static implicit operator int(JsonValue value)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      return Convert.ToInt32(((JsonPrimitive) value).Value, (IFormatProvider) NumberFormatInfo.InvariantInfo);
    }

    public static implicit operator long(JsonValue value)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      return Convert.ToInt64(((JsonPrimitive) value).Value, (IFormatProvider) NumberFormatInfo.InvariantInfo);
    }

    public static implicit operator sbyte(JsonValue value)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      return Convert.ToSByte(((JsonPrimitive) value).Value, (IFormatProvider) NumberFormatInfo.InvariantInfo);
    }

    public static implicit operator short(JsonValue value)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      return Convert.ToInt16(((JsonPrimitive) value).Value, (IFormatProvider) NumberFormatInfo.InvariantInfo);
    }

    public static implicit operator string(JsonValue value)
    {
      return value?.ToString(true);
    }

    public static implicit operator uint(JsonValue value)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      return Convert.ToUInt32(((JsonPrimitive) value).Value, (IFormatProvider) NumberFormatInfo.InvariantInfo);
    }

    public static implicit operator ulong(JsonValue value)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      return Convert.ToUInt64(((JsonPrimitive) value).Value, (IFormatProvider) NumberFormatInfo.InvariantInfo);
    }

    public static implicit operator ushort(JsonValue value)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      return Convert.ToUInt16(((JsonPrimitive) value).Value, (IFormatProvider) NumberFormatInfo.InvariantInfo);
    }

    public static implicit operator DateTime(JsonValue value)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      return (DateTime) ((JsonPrimitive) value).Value;
    }

    public static implicit operator DateTimeOffset(JsonValue value)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      return (DateTimeOffset) ((JsonPrimitive) value).Value;
    }

    public static implicit operator TimeSpan(JsonValue value)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      return (TimeSpan) ((JsonPrimitive) value).Value;
    }

    public static implicit operator Guid(JsonValue value)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      return (Guid) ((JsonPrimitive) value).Value;
    }

    public static implicit operator Uri(JsonValue value)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      return (Uri) ((JsonPrimitive) value).Value;
    }
  }
}
