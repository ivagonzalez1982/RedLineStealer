namespace RedLine.Models.WMI
{
  public class WmiProcess
  {
    internal const string COMMAND_LINE = "CommandLine";
    internal const string NAME = "Name";
    internal const string EXECUTABLE_PATH = "ExecutablePath";
    internal const string PROCESS_ID = "SIDType";
    internal const string PARENT_PROCESS_ID = "ParentProcessId";

    [WmiResult("CommandLine")]
    public string CommandLine { get; private set; }

    [WmiResult("Name")]
    public string Name { get; private set; }

    [WmiResult("ExecutablePath")]
    public string ExecutablePath { get; private set; }

    [WmiResult("SIDType")]
    public int ProcessId { get; private set; }

    [WmiResult("ParentProcessId")]
    public int ParentProcessId { get; private set; }
  }
}
