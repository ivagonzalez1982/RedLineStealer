using System.Collections.Generic;
using System.Text;

namespace RedLine.Models.Gecko
{
  public class Asn1Object
  {
    public Asn1Type ObjectType { get; set; }

    public byte[] ObjectData { get; set; }

    public int ObjectLength { get; set; }

    public List<Asn1Object> Objects { get; set; }

    public Asn1Object()
    {
      this.Objects = new List<Asn1Object>();
    }

    public override string ToString()
    {
      StringBuilder stringBuilder1 = new StringBuilder();
      StringBuilder stringBuilder2 = new StringBuilder();
      switch (this.ObjectType)
      {
        case Asn1Type.Integer:
          foreach (byte num in this.ObjectData)
            stringBuilder2.AppendFormat("{0:X2}", (object) num);
          stringBuilder1.Append("\tINTEGER ").Append((object) stringBuilder2).AppendLine();
          break;
        case Asn1Type.OctetString:
          foreach (byte num in this.ObjectData)
            stringBuilder2.AppendFormat("{0:X2}", (object) num);
          stringBuilder1.Append("\tOCTETSTRING ").AppendLine(stringBuilder2.ToString());
          break;
        case Asn1Type.ObjectIdentifier:
          foreach (byte num in this.ObjectData)
            stringBuilder2.AppendFormat("{0:X2}", (object) num);
          stringBuilder1.Append("\tOBJECTIDENTIFIER ").AppendLine(stringBuilder2.ToString());
          break;
        case Asn1Type.Sequence:
          stringBuilder1.AppendLine("SEQUENCE {");
          break;
      }
      foreach (Asn1Object asn1Object in this.Objects)
        stringBuilder1.Append(asn1Object.ToString());
      if (this.ObjectType == Asn1Type.Sequence)
        stringBuilder1.AppendLine("}");
      stringBuilder2.Remove(0, stringBuilder2.Length - 1);
      return stringBuilder1.ToString();
    }
  }
}
