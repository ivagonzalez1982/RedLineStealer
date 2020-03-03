using System.Runtime.Serialization;

namespace RedLine.Models.Browsers
{
  [DataContract(Name = "Autofill", Namespace = "v1/Models")]
  public class Autofill
  {
    [DataMember(Name = "Name")]
    public string Name { get; set; }

    [DataMember(Name = "Value")]
    public string Value { get; set; }
  }
}
