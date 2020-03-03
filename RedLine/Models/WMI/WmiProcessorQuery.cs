namespace RedLine.Models.WMI
{
  public class WmiProcessorQuery : WmiQueryBase
  {
    public WmiProcessorQuery()
      : base("Win32_Processor", (string) null, new string[9]
      {
        "Manufacturer",
        "Caption",
        "Name",
        "ProcessorId",
        "NumberOfCores",
        "NumberOfLogicalProcessors",
        "L2CacheSize",
        "L3CacheSize",
        "SocketDesignation"
      })
    {
    }
  }
}
