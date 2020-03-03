using System;

namespace RedLine.Logic.Extensions
{
  public static class Extensions
  {
    public static T ForceTo<T>(this object @this)
    {
      return (T) Convert.ChangeType(@this, typeof (T));
    }

    public static string StripQuotes(this string value)
    {
      return value.Replace("\"", string.Empty);
    }
  }
}
