namespace RedLine.Models
{
  public class ProtectionSettings
  {
    public int Check_Timeout = 1000;
    public string[] Snooper_Titles = new string[6]
    {
      "wireshark",
      "ilspy",
      "dnspy",
      "ollydbg",
      "de4dot",
      "megadumper"
    };

    public bool VirtualMachine { get; set; }

    public bool Debugging { get; set; }

    public bool Emulation { get; set; }

    public bool Snooping { get; set; }

    public bool Sandbox { get; set; }
  }
}
