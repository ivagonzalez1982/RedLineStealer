namespace RedLine.Models.WMI
{
  public class WmiAntivirus
  {
    internal const string DISPLAYNAME = "displayName";
    internal const string PATH = "pathToSignedProductExe";

    [WmiResult("displayName")]
    public string DisplayName { get; private set; }

    [WmiResult("pathToSignedProductExe")]
    public string Path { get; private set; }
  }
}
