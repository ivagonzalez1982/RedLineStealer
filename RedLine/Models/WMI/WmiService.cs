using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management;
using System.Reflection;

namespace RedLine.Models.WMI
{
  public class WmiService : IWmiService
  {
    private static TResult Extract<TResult>(ManagementBaseObject managementObject) where TResult : class, new()
    {
      TResult result = new TResult();
      foreach (PropertyInfo property in typeof (TResult).GetProperties())
      {
        WmiResultAttribute customAttribute = (WmiResultAttribute) Attribute.GetCustomAttribute((MemberInfo) property, typeof (WmiResultAttribute));
        if (customAttribute != null)
        {
          object obj1 = managementObject.Properties[customAttribute.PropertyName].Value;
          Type type = Nullable.GetUnderlyingType(property.PropertyType);
          if ((object) type == null)
            type = property.PropertyType;
          Type conversionType = type;
          object obj2 = obj1 != null ? (!(conversionType == typeof (DateTime)) ? (!(conversionType == typeof (Guid)) ? Convert.ChangeType(managementObject.Properties[customAttribute.PropertyName].Value, conversionType) : (object) Guid.Parse(obj1.ToString())) : (object) ManagementDateTimeConverter.ToDateTime(obj1.ToString()).ToUniversalTime()) : (object) null;
          property.SetValue((object) result, obj2, (object[]) null);
        }
      }
      return result;
    }

    private ManagementObjectCollection QueryAll(
      SelectQuery selectQuery,
      ManagementObjectSearcher searcher = null)
    {
      searcher = searcher ?? new ManagementObjectSearcher();
      searcher.Query = (ObjectQuery) selectQuery;
      return searcher.Get();
    }

    private ManagementBaseObject QueryFirst(
      SelectQuery selectQuery,
      ManagementObjectSearcher searcher = null)
    {
      return this.QueryAll(selectQuery, searcher).Cast<ManagementBaseObject>().FirstOrDefault<ManagementBaseObject>();
    }

    public TResult QueryFirst<TResult>(WmiQueryBase wmiQuery) where TResult : class, new()
    {
      ManagementBaseObject managementObject = this.QueryFirst(wmiQuery.SelectQuery, (ManagementObjectSearcher) null);
      if (managementObject != null)
        return WmiService.Extract<TResult>(managementObject);
      return default (TResult);
    }

    public ReadOnlyCollection<TResult> QueryAll<TResult>(
      WmiQueryBase wmiQuery,
      ManagementObjectSearcher searcher = null)
      where TResult : class, new()
    {
      ManagementObjectCollection source = this.QueryAll(wmiQuery.SelectQuery, searcher);
      return new ReadOnlyCollection<TResult>(source != null ? (IList<TResult>) source.Cast<ManagementBaseObject>().Select<ManagementBaseObject, TResult>(new Func<ManagementBaseObject, TResult>(WmiService.Extract<TResult>)).ToList<TResult>() : (IList<TResult>) null);
    }
  }
}
