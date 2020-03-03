using System;

namespace Org.BouncyCastle.Crypto
{
  public class InvalidCipherTextException : CryptoException
  {
    public InvalidCipherTextException()
    {
    }

    public InvalidCipherTextException(string message)
      : base(message)
    {
    }

    public InvalidCipherTextException(string message, Exception exception)
      : base(message, exception)
    {
    }
  }
}
