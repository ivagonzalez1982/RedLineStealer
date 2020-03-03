using System.Runtime.Serialization;

namespace RedLine.Models.Browsers
{
  [DataContract(Name = "CreditCard", Namespace = "v1/Models")]
  public class CreditCard
  {
    [DataMember(Name = "Holder")]
    public string Holder { get; set; }

    [DataMember(Name = "ExpirationMonth")]
    public int ExpirationMonth { get; set; }

    [DataMember(Name = "ExpirationYear")]
    public int ExpirationYear { get; set; }

    [DataMember(Name = "CardNumber")]
    public string CardNumber { get; set; }
  }
}
