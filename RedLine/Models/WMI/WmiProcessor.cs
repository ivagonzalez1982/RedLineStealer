namespace RedLine.Models.WMI
{
  public class WmiProcessor
  {
    internal const string MANUFACTURER = "Manufacturer";
    internal const string CAPTION = "Caption";
    internal const string NAME = "Name";
    internal const string PROCESSOR_ID = "ProcessorId";
    internal const string NUM_OF_CORES = "NumberOfCores";
    internal const string NUM_OF_LOGICAL_PROCESSORS = "NumberOfLogicalProcessors";
    internal const string L2_CACHE_SIZE = "L2CacheSize";
    internal const string L3_CACHE_SIZE = "L3CacheSize";
    internal const string SOCKET_DESIGNATION = "SocketDesignation";

    [WmiResult("Manufacturer")]
    public string Manufacturer { get; private set; }

    [WmiResult("Caption")]
    public string Caption { get; private set; }

    [WmiResult("Name")]
    public string Name { get; private set; }

    [WmiResult("ProcessorId")]
    public string ProcessorId { get; private set; }

    [WmiResult("NumberOfCores")]
    public int? NumberOfCores { get; private set; }

    [WmiResult("NumberOfLogicalProcessors")]
    public int? NumberOfLogicalProcessors { get; private set; }

    [WmiResult("L2CacheSize")]
    public int? L2CacheSize { get; private set; }

    [WmiResult("L3CacheSize")]
    public int? L3CacheSize { get; private set; }

    [WmiResult("SocketDesignation")]
    public string SocketDesignation { get; private set; }
  }
}
