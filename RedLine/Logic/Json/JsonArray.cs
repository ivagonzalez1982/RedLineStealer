using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace RedLine.Logic.Json
{
  public class JsonArray : JsonValue, IList<JsonValue>, ICollection<JsonValue>, IEnumerable<JsonValue>, IEnumerable
  {
    private List<JsonValue> list;

    public JsonArray(params JsonValue[] items)
    {
      this.list = new List<JsonValue>();
      this.AddRange(items);
    }

    public JsonArray(IEnumerable<JsonValue> items)
    {
      if (items == null)
        throw new ArgumentNullException(nameof (items));
      this.list = new List<JsonValue>(items);
    }

    public override int Count
    {
      get
      {
        return this.list.Count;
      }
    }

    public bool IsReadOnly
    {
      get
      {
        return false;
      }
    }

    public override sealed JsonValue this[int index]
    {
      get
      {
        return this.list[index];
      }
      set
      {
        this.list[index] = value;
      }
    }

    public override JsonType JsonType
    {
      get
      {
        return JsonType.Array;
      }
    }

    public void Add(JsonValue item)
    {
      if (item == null)
        throw new ArgumentNullException(nameof (item));
      this.list.Add(item);
    }

    public void AddRange(IEnumerable<JsonValue> items)
    {
      if (items == null)
        throw new ArgumentNullException(nameof (items));
      this.list.AddRange(items);
    }

    public void AddRange(params JsonValue[] items)
    {
      if (items == null)
        return;
      this.list.AddRange((IEnumerable<JsonValue>) items);
    }

    public void Clear()
    {
      this.list.Clear();
    }

    public bool Contains(JsonValue item)
    {
      return this.list.Contains(item);
    }

    public void CopyTo(JsonValue[] array, int arrayIndex)
    {
      this.list.CopyTo(array, arrayIndex);
    }

    public int IndexOf(JsonValue item)
    {
      return this.list.IndexOf(item);
    }

    public void Insert(int index, JsonValue item)
    {
      this.list.Insert(index, item);
    }

    public bool Remove(JsonValue item)
    {
      return this.list.Remove(item);
    }

    public void RemoveAt(int index)
    {
      this.list.RemoveAt(index);
    }

    public override void Save(Stream stream, bool parsing)
    {
      if (stream == null)
        throw new ArgumentNullException(nameof (stream));
      stream.WriteByte((byte) 91);
      for (int index = 0; index < this.list.Count; ++index)
      {
        JsonValue jsonValue = this.list[index];
        if (jsonValue != null)
        {
          jsonValue.Save(stream, parsing);
        }
        else
        {
          stream.WriteByte((byte) 110);
          stream.WriteByte((byte) 117);
          stream.WriteByte((byte) 108);
          stream.WriteByte((byte) 108);
        }
        if (index < this.Count - 1)
        {
          stream.WriteByte((byte) 44);
          stream.WriteByte((byte) 32);
        }
      }
      stream.WriteByte((byte) 93);
    }

    IEnumerator<JsonValue> IEnumerable<JsonValue>.GetEnumerator()
    {
      return (IEnumerator<JsonValue>) this.list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.list.GetEnumerator();
    }
  }
}
