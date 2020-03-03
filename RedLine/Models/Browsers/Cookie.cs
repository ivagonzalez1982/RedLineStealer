using System.Runtime.Serialization;

namespace RedLine.Models.Browsers
{
  [DataContract(Name = "Cookie", Namespace = "v1/Models")]
  public class Cookie
  {
    [DataMember(Name = "Host")]
    public string Host { get; set; }

    [DataMember(Name = "Http")]
    public bool Http { get; set; }

    [DataMember(Name = "Path")]
    public string Path { get; set; }

    [DataMember(Name = "Secure")]
    public bool Secure { get; set; }

    [DataMember(Name = "Expires")]
    public string Expires { get; set; }

    [DataMember(Name = "Name")]
    public string Name { get; set; }

    [DataMember(Name = "Value")]
    public string Value { get; set; }
  }
}
