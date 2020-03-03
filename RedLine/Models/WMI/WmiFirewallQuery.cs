namespace RedLine.Models.WMI
{
  public class WmiFirewallQuery : WmiQueryBase
  {
    public WmiFirewallQuery()
      : base("FirewallProduct", (string) null, new string[2]
      {
        "displayName",
        "pathToSignedProductExe"
      })
    {
    }
  }
}
