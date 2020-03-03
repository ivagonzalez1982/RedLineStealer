namespace RedLine.Models.WMI
{
  public class WmiNetworkAdapterQuery : WmiQueryBase
  {
    private static readonly string[] COLUMN_NAMES = new string[3]
    {
      "GUID",
      "MACAddress",
      "PNPDeviceID"
    };

    public WmiNetworkAdapterQuery(WmiNetworkAdapterType adapterType = WmiNetworkAdapterType.All)
      : base("Win32_NetworkAdapter", (string) null, WmiNetworkAdapterQuery.COLUMN_NAMES)
    {
      if (adapterType == WmiNetworkAdapterType.Physical)
      {
        this.SelectQuery.Condition = "PhysicalAdapter=1";
      }
      else
      {
        if (adapterType != WmiNetworkAdapterType.Virtual)
          return;
        this.SelectQuery.Condition = "PhysicalAdapter=0";
      }
    }
  }
}
