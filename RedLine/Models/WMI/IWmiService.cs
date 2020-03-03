using System.Collections.ObjectModel;
using System.Management;

namespace RedLine.Models.WMI
{
  public interface IWmiService
  {
    TResult QueryFirst<TResult>(WmiQueryBase wmiQuery) where TResult : class, new();

    ReadOnlyCollection<TResult> QueryAll<TResult>(
      WmiQueryBase wmiQuery,
      ManagementObjectSearcher searcher)
      where TResult : class, new();
  }
}
