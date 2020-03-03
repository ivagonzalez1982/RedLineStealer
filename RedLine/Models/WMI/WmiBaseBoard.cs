namespace RedLine.Models.WMI
{
  public class WmiBaseBoard
  {
    internal const string MANUFACTURER = "Manufacturer";
    internal const string PRODUCT = "Product";
    internal const string SERIAL_NUMBER = "SerialNumber";

    [WmiResult("Manufacturer")]
    public string Manufacturer { get; private set; }

    [WmiResult("Product")]
    public string Product { get; private set; }

    [WmiResult("SerialNumber")]
    public string SerialNumber { get; private set; }
  }
}
