using Org.BouncyCastle.Crypto.Utilities;

namespace Org.BouncyCastle.Crypto.Modes.Gcm
{
  public class Tables8kGcmMultiplier : IGcmMultiplier
  {
    private readonly uint[][][] M = new uint[32][][];

    public void Init(byte[] H)
    {
      this.M[0] = new uint[16][];
      this.M[1] = new uint[16][];
      this.M[0][0] = new uint[4];
      this.M[1][0] = new uint[4];
      this.M[1][8] = GcmUtilities.AsUints(H);
      for (int index = 4; index >= 1; index >>= 1)
      {
        uint[] x = (uint[]) this.M[1][index + index].Clone();
        GcmUtilities.MultiplyP(x);
        this.M[1][index] = x;
      }
      uint[] x1 = (uint[]) this.M[1][1].Clone();
      GcmUtilities.MultiplyP(x1);
      this.M[0][8] = x1;
      for (int index = 4; index >= 1; index >>= 1)
      {
        uint[] x2 = (uint[]) this.M[0][index + index].Clone();
        GcmUtilities.MultiplyP(x2);
        this.M[0][index] = x2;
      }
      int index1 = 0;
label_7:
      do
      {
        for (int index2 = 2; index2 < 16; index2 += index2)
        {
          for (int index3 = 1; index3 < index2; ++index3)
          {
            uint[] block = (uint[]) this.M[index1][index2].Clone();
            GcmUtilities.Xor(block, this.M[index1][index3]);
            this.M[index1][index2 + index3] = block;
          }
        }
        if (++index1 == 32)
          return;
      }
      while (index1 <= 1);
      this.M[index1] = new uint[16][];
      this.M[index1][0] = new uint[4];
      for (int index2 = 8; index2 > 0; index2 >>= 1)
      {
        uint[] x2 = (uint[]) this.M[index1 - 2][index2].Clone();
        GcmUtilities.MultiplyP8(x2);
        this.M[index1][index2] = x2;
      }
      goto label_7;
    }

    public void MultiplyH(byte[] x)
    {
      uint[] numArray1 = new uint[4];
      for (int index = 15; index >= 0; --index)
      {
        uint[] numArray2 = this.M[index + index][(int) x[index] & 15];
        numArray1[0] ^= numArray2[0];
        numArray1[1] ^= numArray2[1];
        numArray1[2] ^= numArray2[2];
        numArray1[3] ^= numArray2[3];
        uint[] numArray3 = this.M[index + index + 1][((int) x[index] & 240) >> 4];
        numArray1[0] ^= numArray3[0];
        numArray1[1] ^= numArray3[1];
        numArray1[2] ^= numArray3[2];
        numArray1[3] ^= numArray3[3];
      }
      Pack.UInt32_To_BE(numArray1[0], x, 0);
      Pack.UInt32_To_BE(numArray1[1], x, 4);
      Pack.UInt32_To_BE(numArray1[2], x, 8);
      Pack.UInt32_To_BE(numArray1[3], x, 12);
    }
  }
}
