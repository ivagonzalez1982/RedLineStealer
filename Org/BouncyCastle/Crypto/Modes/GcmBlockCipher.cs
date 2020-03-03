using Org.BouncyCastle.Crypto.Modes.Gcm;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Utilities;
using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Modes
{
  public class GcmBlockCipher : IAeadBlockCipher
  {
    private static readonly byte[] Zeroes = new byte[16];
    private const int BlockSize = 16;
    private readonly IBlockCipher cipher;
    private readonly IGcmMultiplier multiplier;
    private bool forEncryption;
    private int macSize;
    private byte[] nonce;
    private byte[] A;
    private KeyParameter keyParam;
    private byte[] H;
    private byte[] initS;
    private byte[] J0;
    private byte[] bufBlock;
    private byte[] macBlock;
    private byte[] S;
    private byte[] counter;
    private int bufOff;
    private ulong totalLength;

    public GcmBlockCipher(IBlockCipher c)
      : this(c, (IGcmMultiplier) null)
    {
    }

    public GcmBlockCipher(IBlockCipher c, IGcmMultiplier m)
    {
      if (c.GetBlockSize() != 16)
        throw new ArgumentException("cipher required with a block size of " + (object) 16 + ".");
      if (m == null)
        m = (IGcmMultiplier) new Tables8kGcmMultiplier();
      this.cipher = c;
      this.multiplier = m;
    }

    public virtual string AlgorithmName
    {
      get
      {
        return this.cipher.AlgorithmName + "/GCM";
      }
    }

    public virtual int GetBlockSize()
    {
      return 16;
    }

    public virtual void Init(bool forEncryption, ICipherParameters parameters)
    {
      this.forEncryption = forEncryption;
      this.macBlock = (byte[]) null;
      if (parameters is AeadParameters)
      {
        AeadParameters aeadParameters = (AeadParameters) parameters;
        this.nonce = aeadParameters.GetNonce();
        this.A = aeadParameters.GetAssociatedText();
        int macSize = aeadParameters.MacSize;
        if (macSize < 96 || macSize > 128 || macSize % 8 != 0)
          throw new ArgumentException("Invalid value for MAC size: " + (object) macSize);
        this.macSize = macSize / 8;
        this.keyParam = aeadParameters.Key;
      }
      else
      {
        if (!(parameters is ParametersWithIV))
          throw new ArgumentException("invalid parameters passed to GCM");
        ParametersWithIV parametersWithIv = (ParametersWithIV) parameters;
        this.nonce = parametersWithIv.GetIV();
        this.A = (byte[]) null;
        this.macSize = 16;
        this.keyParam = (KeyParameter) parametersWithIv.Parameters;
      }
      this.bufBlock = new byte[forEncryption ? 16 : 16 + this.macSize];
      if (this.nonce == null || this.nonce.Length < 1)
        throw new ArgumentException("IV must be at least 1 byte");
      if (this.A == null)
        this.A = new byte[0];
      this.cipher.Init(true, (ICipherParameters) this.keyParam);
      this.H = new byte[16];
      this.cipher.ProcessBlock(this.H, 0, this.H, 0);
      this.multiplier.Init(this.H);
      this.initS = this.gHASH(this.A);
      if (this.nonce.Length == 12)
      {
        this.J0 = new byte[16];
        Array.Copy((Array) this.nonce, 0, (Array) this.J0, 0, this.nonce.Length);
        this.J0[15] = (byte) 1;
      }
      else
      {
        this.J0 = this.gHASH(this.nonce);
        byte[] numArray = new byte[16];
        GcmBlockCipher.packLength((ulong) this.nonce.Length * 8UL, numArray, 8);
        GcmUtilities.Xor(this.J0, numArray);
        this.multiplier.MultiplyH(this.J0);
      }
      this.S = Arrays.Clone(this.initS);
      this.counter = Arrays.Clone(this.J0);
      this.bufOff = 0;
      this.totalLength = 0UL;
    }

    public virtual byte[] GetMac()
    {
      return Arrays.Clone(this.macBlock);
    }

    public virtual int GetOutputSize(int len)
    {
      if (this.forEncryption)
        return len + this.bufOff + this.macSize;
      return len + this.bufOff - this.macSize;
    }

    public virtual int GetUpdateOutputSize(int len)
    {
      return (len + this.bufOff) / 16 * 16;
    }

    public virtual int ProcessByte(byte input, byte[] output, int outOff)
    {
      return this.Process(input, output, outOff);
    }

    public virtual int ProcessBytes(byte[] input, int inOff, int len, byte[] output, int outOff)
    {
      int num = 0;
      for (int index = 0; index != len; ++index)
      {
        this.bufBlock[this.bufOff++] = input[inOff + index];
        if (this.bufOff == this.bufBlock.Length)
        {
          this.gCTRBlock(this.bufBlock, 16, output, outOff + num);
          if (!this.forEncryption)
            Array.Copy((Array) this.bufBlock, 16, (Array) this.bufBlock, 0, this.macSize);
          this.bufOff = this.bufBlock.Length - 16;
          num += 16;
        }
      }
      return num;
    }

    private int Process(byte input, byte[] output, int outOff)
    {
      this.bufBlock[this.bufOff++] = input;
      if (this.bufOff != this.bufBlock.Length)
        return 0;
      this.gCTRBlock(this.bufBlock, 16, output, outOff);
      if (!this.forEncryption)
        Array.Copy((Array) this.bufBlock, 16, (Array) this.bufBlock, 0, this.macSize);
      this.bufOff = this.bufBlock.Length - 16;
      return 16;
    }

    public int DoFinal(byte[] output, int outOff)
    {
      int bufOff = this.bufOff;
      if (!this.forEncryption)
      {
        if (bufOff < this.macSize)
          throw new InvalidCipherTextException("data too short");
        bufOff -= this.macSize;
      }
      if (bufOff > 0)
      {
        byte[] buf = new byte[16];
        Array.Copy((Array) this.bufBlock, 0, (Array) buf, 0, bufOff);
        this.gCTRBlock(buf, bufOff, output, outOff);
      }
      byte[] numArray1 = new byte[16];
      GcmBlockCipher.packLength((ulong) this.A.Length * 8UL, numArray1, 0);
      GcmBlockCipher.packLength(this.totalLength * 8UL, numArray1, 8);
      GcmUtilities.Xor(this.S, numArray1);
      this.multiplier.MultiplyH(this.S);
      byte[] numArray2 = new byte[16];
      this.cipher.ProcessBlock(this.J0, 0, numArray2, 0);
      GcmUtilities.Xor(numArray2, this.S);
      int num = bufOff;
      this.macBlock = new byte[this.macSize];
      Array.Copy((Array) numArray2, 0, (Array) this.macBlock, 0, this.macSize);
      if (this.forEncryption)
      {
        Array.Copy((Array) this.macBlock, 0, (Array) output, outOff + this.bufOff, this.macSize);
        num += this.macSize;
      }
      else
      {
        byte[] b = new byte[this.macSize];
        Array.Copy((Array) this.bufBlock, bufOff, (Array) b, 0, this.macSize);
        if (!Arrays.ConstantTimeAreEqual(this.macBlock, b))
          throw new InvalidCipherTextException("mac check in GCM failed");
      }
      this.Reset(false);
      return num;
    }

    public virtual void Reset()
    {
      this.Reset(true);
    }

    private void Reset(bool clearMac)
    {
      this.S = Arrays.Clone(this.initS);
      this.counter = Arrays.Clone(this.J0);
      this.bufOff = 0;
      this.totalLength = 0UL;
      if (this.bufBlock != null)
        Array.Clear((Array) this.bufBlock, 0, this.bufBlock.Length);
      if (clearMac)
        this.macBlock = (byte[]) null;
      this.cipher.Reset();
    }

    private void gCTRBlock(byte[] buf, int bufCount, byte[] output, int outOff)
    {
      int index1 = 15;
      while (index1 >= 12 && ++this.counter[index1] == (byte) 0)
        --index1;
      byte[] outBuf = new byte[16];
      this.cipher.ProcessBlock(this.counter, 0, outBuf, 0);
      byte[] val;
      if (this.forEncryption)
      {
        Array.Copy((Array) GcmBlockCipher.Zeroes, bufCount, (Array) outBuf, bufCount, 16 - bufCount);
        val = outBuf;
      }
      else
        val = buf;
      for (int index2 = bufCount - 1; index2 >= 0; --index2)
      {
        outBuf[index2] ^= buf[index2];
        output[outOff + index2] = outBuf[index2];
      }
      GcmUtilities.Xor(this.S, val);
      this.multiplier.MultiplyH(this.S);
      this.totalLength += (ulong) bufCount;
    }

    private byte[] gHASH(byte[] b)
    {
      byte[] numArray = new byte[16];
      for (int sourceIndex = 0; sourceIndex < b.Length; sourceIndex += 16)
      {
        byte[] val = new byte[16];
        int length = Math.Min(b.Length - sourceIndex, 16);
        Array.Copy((Array) b, sourceIndex, (Array) val, 0, length);
        GcmUtilities.Xor(numArray, val);
        this.multiplier.MultiplyH(numArray);
      }
      return numArray;
    }

    private static void packLength(ulong len, byte[] bs, int off)
    {
      Pack.UInt32_To_BE((uint) (len >> 32), bs, off);
      Pack.UInt32_To_BE((uint) len, bs, off + 4);
    }
  }
}
