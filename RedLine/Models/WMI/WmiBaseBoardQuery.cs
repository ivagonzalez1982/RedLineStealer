namespace RedLine.Models.WMI
{
  public class WmiBaseBoardQuery : WmiQueryBase
  {
    public WmiBaseBoardQuery()
      : base("Win32_BaseBoard", (string) null, new string[3]
      {
        "Manufacturer",
        "Product",
        "SerialNumber"
      })
    {
    }
  }
}
