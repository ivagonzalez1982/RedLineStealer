using Org.BouncyCastle.Crypto.Utilities;
using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Modes.Gcm
{
  internal abstract class GcmUtilities
  {
    internal static byte[] OneAsBytes()
    {
      byte[] numArray = new byte[16];
      numArray[0] = (byte) 128;
      return numArray;
    }

    internal static uint[] OneAsUints()
    {
      return new uint[4]{ 2147483648U, 0U, 0U, 0U };
    }

    internal static uint[] AsUints(byte[] bs)
    {
      return new uint[4]
      {
        Pack.BE_To_UInt32(bs, 0),
        Pack.BE_To_UInt32(bs, 4),
        Pack.BE_To_UInt32(bs, 8),
        Pack.BE_To_UInt32(bs, 12)
      };
    }

    internal static void Multiply(byte[] block, byte[] val)
    {
      byte[] numArray = Arrays.Clone(block);
      byte[] block1 = new byte[16];
      for (int index1 = 0; index1 < 16; ++index1)
      {
        byte num1 = val[index1];
        for (int index2 = 7; index2 >= 0; --index2)
        {
          if (((int) num1 & 1 << index2) != 0)
            GcmUtilities.Xor(block1, numArray);
          int num2 = ((uint) numArray[15] & 1U) > 0U ? 1 : 0;
          GcmUtilities.ShiftRight(numArray);
          if (num2 != 0)
            numArray[0] ^= (byte) 225;
        }
      }
      Array.Copy((Array) block1, 0, (Array) block, 0, 16);
    }

    internal static void MultiplyP(uint[] x)
    {
      int num = (x[3] & 1U) > 0U ? 1 : 0;
      GcmUtilities.ShiftRight(x);
      if (num == 0)
        return;
      x[0] ^= 3774873600U;
    }

    internal static void MultiplyP8(uint[] x)
    {
      uint num = x[3];
      GcmUtilities.ShiftRightN(x, 8);
      for (int index = 7; index >= 0; --index)
      {
        if (((long) num & (long) (1 << index)) != 0L)
          x[0] ^= 3774873600U >> 7 - index;
      }
    }

    internal static void ShiftRight(byte[] block)
    {
      int index = 0;
      byte num1 = 0;
      while (true)
      {
        byte num2 = block[index];
        block[index] = (byte) ((uint) num2 >> 1 | (uint) num1);
        if (++index != 16)
          num1 = (byte) ((uint) num2 << 7);
        else
          break;
      }
    }

    internal static void ShiftRight(uint[] block)
    {
      int index = 0;
      uint num1 = 0;
      while (true)
      {
        uint num2 = block[index];
        block[index] = num2 >> 1 | num1;
        if (++index != 4)
          num1 = num2 << 31;
        else
          break;
      }
    }

    internal static void ShiftRightN(uint[] block, int n)
    {
      int index = 0;
      uint num1 = 0;
      while (true)
      {
        uint num2 = block[index];
        block[index] = num2 >> n | num1;
        if (++index != 4)
          num1 = num2 << 32 - n;
        else
          break;
      }
    }

    internal static void Xor(byte[] block, byte[] val)
    {
      for (int index = 15; index >= 0; --index)
        block[index] ^= val[index];
    }

    internal static void Xor(uint[] block, uint[] val)
    {
      for (int index = 3; index >= 0; --index)
        block[index] ^= val[index];
    }
  }
}
