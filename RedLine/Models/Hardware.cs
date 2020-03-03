using System.Runtime.Serialization;

namespace RedLine.Models
{
  [DataContract(Name = "Hardware", Namespace = "v1/Models")]
  public class Hardware
  {
    [DataMember(Name = "Caption")]
    public string Caption { get; set; }

    [DataMember(Name = "Parameter")]
    public string Parameter { get; set; }

    [DataMember(Name = "HardType")]
    public HardwareType HardType { get; set; }

    public override string ToString()
    {
      return "Name: " + this.Caption + "," + (this.HardType == HardwareType.Processor ? " " + this.Parameter + " Cores" : " " + this.Parameter + " bytes");
    }
  }
}
