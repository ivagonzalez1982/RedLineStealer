using System;

namespace RedLine.Models.WMI
{
  [AttributeUsage(AttributeTargets.Property)]
  public class WmiResultAttribute : Attribute
  {
    public WmiResultAttribute(string propertyName)
    {
      this.PropertyName = propertyName;
    }

    public string PropertyName { get; }
  }
}
