namespace RedLine.Models.WMI
{
  public class WmiDiskDrive
  {
    internal const string DEVICE_ID = "DeviceID";
    internal const string MEDIA_TYPE = "MediaType";
    internal const string MODEL = "Model";
    internal const string PNP_DEVICE_ID = "PNPDeviceID";
    internal const string SERIAL_NUMBER = "SerialNumber";

    [WmiResult("DeviceID")]
    public string DeviceId { get; private set; }

    [WmiResult("MediaType")]
    public string MediaType { get; private set; }

    [WmiResult("Model")]
    public string Model { get; private set; }

    [WmiResult("PNPDeviceID")]
    public string PnpDeviceId { get; private set; }

    [WmiResult("SerialNumber")]
    public string SerialNumber { get; private set; }
  }
}
