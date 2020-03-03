using System.Management;

namespace RedLine.Models.WMI
{
  public class WmiQueryBase
  {
    private readonly SelectQuery _selectQuery;

    protected WmiQueryBase(string className, string condition = null, string[] selectedProperties = null)
    {
      this._selectQuery = new SelectQuery(className, condition, selectedProperties);
    }

    internal SelectQuery SelectQuery
    {
      get
      {
        return this._selectQuery;
      }
    }
  }
}
