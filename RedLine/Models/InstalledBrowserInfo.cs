using System.Runtime.Serialization;

namespace RedLine.Models
{
  [DataContract(Name = "InstalledBrowserInfo", Namespace = "v1/Models")]
  public class InstalledBrowserInfo
  {
    [DataMember(Name = "Name")]
    public string Name { get; set; }

    [DataMember(Name = "Version")]
    public string Version { get; set; }

    [DataMember(Name = "Path")]
    public string Path { get; set; }
  }
}
