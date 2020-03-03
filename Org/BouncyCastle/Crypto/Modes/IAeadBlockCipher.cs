namespace Org.BouncyCastle.Crypto.Modes
{
  public interface IAeadBlockCipher
  {
    string AlgorithmName { get; }

    void Init(bool forEncryption, ICipherParameters parameters);

    int GetBlockSize();

    int ProcessByte(byte input, byte[] outBytes, int outOff);

    int ProcessBytes(byte[] inBytes, int inOff, int len, byte[] outBytes, int outOff);

    int DoFinal(byte[] outBytes, int outOff);

    byte[] GetMac();

    int GetUpdateOutputSize(int len);

    int GetOutputSize(int len);

    void Reset();
  }
}
