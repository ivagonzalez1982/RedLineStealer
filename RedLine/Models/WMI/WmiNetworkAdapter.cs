namespace RedLine.Models.WMI
{
  public class WmiNetworkAdapter
  {
    internal const string PNP_DEVICE_ID = "PNPDeviceID";
    internal const string GUID = "GUID";
    internal const string MAC_ADDRESS = "MACAddress";

    [WmiResult("PNPDeviceID")]
    public string PnpDeviceId { get; private set; }

    [WmiResult("GUID")]
    public System.Guid? Guid { get; private set; }

    [WmiResult("MACAddress")]
    public string MacAddress { get; private set; }
  }
}
