using System.Runtime.InteropServices;

namespace RedLine.Models.Browsers.Edge
{
  [StructLayout(LayoutKind.Explicit)]
  public struct VAULT_ITEM_ELEMENT
  {
    [FieldOffset(0)]
    public VAULT_SCHEMA_ELEMENT_ID SchemaElementId;
    [FieldOffset(8)]
    public VAULT_ELEMENT_TYPE Type;
  }
}
