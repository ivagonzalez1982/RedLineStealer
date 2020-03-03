namespace RedLine.Models.WMI
{
  public class WmiGraphicCard
  {
    internal const string ADAPTERRAM = "AdapterRAM";
    internal const string CAPTION = "Caption";
    internal const string DESCRIPTION = "Description";
    internal const string NAME = "Name";

    [WmiResult("Description")]
    public string Description { get; private set; }

    [WmiResult("Name")]
    public string Name { get; private set; }

    [WmiResult("Caption")]
    public string Caption { get; private set; }

    [WmiResult("AdapterRAM")]
    public uint AdapterRAM { get; private set; }
  }
}
