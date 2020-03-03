using System.Runtime.Serialization;

namespace RedLine.Models
{
  [DataContract(Name = "LoginPair", Namespace = "v1/Models")]
  public class LoginPair
  {
    [DataMember(Name = "Host")]
    public string Host { get; set; }

    [DataMember(Name = "Login")]
    public string Login { get; set; }

    [DataMember(Name = "Password")]
    public string Password { get; set; }
  }
}
