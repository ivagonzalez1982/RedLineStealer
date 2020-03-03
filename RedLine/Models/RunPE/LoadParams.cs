namespace RedLine.Models.RunPE
{
  public sealed class LoadParams
  {
    public byte[] Body { get; internal set; }

    public string AppPath { get; internal set; }

    public static LoadParams Create(
      byte[] SystemDataCommonIntStorageg,
      string ApplicationPath)
    {
      return new LoadParams()
      {
        AppPath = ApplicationPath,
        Body = SystemDataCommonIntStorageg
      };
    }
  }
}
