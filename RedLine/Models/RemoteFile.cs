using System.Runtime.Serialization;

namespace RedLine.Models
{
  [DataContract(Name = "RemoteFile", Namespace = "v1/Models")]
  public struct RemoteFile
  {
    [DataMember(Name = "FileName")]
    public string FileName { get; set; }

    [DataMember(Name = "SourcePath")]
    public string SourcePath { get; set; }

    [DataMember(Name = "Body")]
    public byte[] Body { get; set; }
  }
}
