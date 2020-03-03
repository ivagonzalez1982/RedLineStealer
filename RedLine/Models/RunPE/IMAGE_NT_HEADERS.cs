using System.Runtime.InteropServices;

namespace RedLine.Models.RunPE
{
  [StructLayout(LayoutKind.Explicit, Size = 248)]
  public struct IMAGE_NT_HEADERS
  {
    [FieldOffset(0)]
    public uint Signature;
    [FieldOffset(4)]
    public IMAGE_FILE_HEADER FileHeader;
    [FieldOffset(24)]
    public IMAGE_OPTIONAL_HEADER OptionalHeader;
  }
}
