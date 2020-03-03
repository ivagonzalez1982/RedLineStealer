using System.Runtime.Serialization;

namespace RedLine.Models
{
  [DataContract(Name = "RemoteTask", Namespace = "v1/Models")]
  public class RemoteTask
  {
    [DataMember(Name = "ID")]
    public int ID { get; set; }

    [DataMember(Name = "Target")]
    public string Target { get; set; }

    [DataMember(Name = "Action")]
    public RemoteTaskAction Action { get; set; }
  }
}
