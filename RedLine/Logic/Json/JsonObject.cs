using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RedLine.Logic.Json
{
  public class JsonObject : JsonValue, IDictionary<string, JsonValue>, ICollection<KeyValuePair<string, JsonValue>>, IEnumerable<KeyValuePair<string, JsonValue>>, IEnumerable
  {
    private SortedDictionary<string, JsonValue> map;

    public JsonObject(params KeyValuePair<string, JsonValue>[] items)
    {
      this.map = new SortedDictionary<string, JsonValue>((IComparer<string>) StringComparer.Ordinal);
      if (items == null)
        return;
      this.AddRange(items);
    }

    public JsonObject(IEnumerable<KeyValuePair<string, JsonValue>> items)
    {
      if (items == null)
        throw new ArgumentNullException(nameof (items));
      this.map = new SortedDictionary<string, JsonValue>((IComparer<string>) StringComparer.Ordinal);
      this.AddRange(items);
    }

    public override int Count
    {
      get
      {
        return this.map.Count;
      }
    }

    public IEnumerator<KeyValuePair<string, JsonValue>> GetEnumerator()
    {
      return (IEnumerator<KeyValuePair<string, JsonValue>>) this.map.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.map.GetEnumerator();
    }

    public override sealed JsonValue this[string key]
    {
      get
      {
        return this.map[key];
      }
      set
      {
        this.map[key] = value;
      }
    }

    public override JsonType JsonType
    {
      get
      {
        return JsonType.Object;
      }
    }

    public ICollection<string> Keys
    {
      get
      {
        return (ICollection<string>) this.map.Keys;
      }
    }

    public ICollection<JsonValue> Values
    {
      get
      {
        return (ICollection<JsonValue>) this.map.Values;
      }
    }

    public void Add(string key, JsonValue value)
    {
      if (key == null)
        throw new ArgumentNullException(nameof (key));
      this.map.Add(key, value);
    }

    public void Add(KeyValuePair<string, JsonValue> pair)
    {
      this.Add(pair.Key, pair.Value);
    }

    public void AddRange(IEnumerable<KeyValuePair<string, JsonValue>> items)
    {
      if (items == null)
        throw new ArgumentNullException(nameof (items));
      foreach (KeyValuePair<string, JsonValue> keyValuePair in items)
        this.map.Add(keyValuePair.Key, keyValuePair.Value);
    }

    public void AddRange(params KeyValuePair<string, JsonValue>[] items)
    {
      this.AddRange((IEnumerable<KeyValuePair<string, JsonValue>>) items);
    }

    public void Clear()
    {
      this.map.Clear();
    }

    bool ICollection<KeyValuePair<string, JsonValue>>.Contains(
      KeyValuePair<string, JsonValue> item)
    {
      return ((ICollection<KeyValuePair<string, JsonValue>>) this.map).Contains(item);
    }

    bool ICollection<KeyValuePair<string, JsonValue>>.Remove(
      KeyValuePair<string, JsonValue> item)
    {
      return ((ICollection<KeyValuePair<string, JsonValue>>) this.map).Remove(item);
    }

    public override bool ContainsKey(string key)
    {
      if (key == null)
        throw new ArgumentNullException(nameof (key));
      return this.map.ContainsKey(key);
    }

    public void CopyTo(KeyValuePair<string, JsonValue>[] array, int arrayIndex)
    {
      this.map.CopyTo(array, arrayIndex);
    }

    public bool Remove(string key)
    {
      if (key == null)
        throw new ArgumentNullException(nameof (key));
      return this.map.Remove(key);
    }

    bool ICollection<KeyValuePair<string, JsonValue>>.IsReadOnly
    {
      get
      {
        return false;
      }
    }

    public override void Save(Stream stream, bool parsing)
    {
      if (stream == null)
        throw new ArgumentNullException(nameof (stream));
      stream.WriteByte((byte) 123);
      foreach (KeyValuePair<string, JsonValue> keyValuePair in this.map)
      {
        stream.WriteByte((byte) 34);
        byte[] bytes = Encoding.UTF8.GetBytes(this.EscapeString(keyValuePair.Key));
        stream.Write(bytes, 0, bytes.Length);
        stream.WriteByte((byte) 34);
        stream.WriteByte((byte) 44);
        stream.WriteByte((byte) 32);
        if (keyValuePair.Value == null)
        {
          stream.WriteByte((byte) 110);
          stream.WriteByte((byte) 117);
          stream.WriteByte((byte) 108);
          stream.WriteByte((byte) 108);
        }
        else
          keyValuePair.Value.Save(stream, parsing);
      }
      stream.WriteByte((byte) 125);
    }

    public bool TryGetValue(string key, out JsonValue value)
    {
      return this.map.TryGetValue(key, out value);
    }
  }
}
