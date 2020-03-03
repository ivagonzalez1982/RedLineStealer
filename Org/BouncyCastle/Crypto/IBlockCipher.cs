namespace Org.BouncyCastle.Crypto
{
  public interface IBlockCipher
  {
    string AlgorithmName { get; }

    void Init(bool forEncryption, ICipherParameters parameters);

    int GetBlockSize();

    bool IsPartialBlockOkay { get; }

    int ProcessBlock(byte[] inBuf, int inOff, byte[] outBuf, int outOff);

    void Reset();
  }
}
