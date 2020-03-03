namespace RedLine.Models.WMI
{
  public class WmiGraphicCardQuery : WmiQueryBase
  {
    public WmiGraphicCardQuery()
      : base("Win32_VideoController", (string) null, new string[4]
      {
        "Name",
        "AdapterRAM",
        "Caption",
        "Description"
      })
    {
    }
  }
}
