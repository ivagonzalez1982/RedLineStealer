using System.Runtime.InteropServices;

namespace RedLine.Models.RunPE
{
  [StructLayout(LayoutKind.Explicit, Size = 40)]
  public struct IMAGE_SECTION_HEADER
  {
    [FieldOffset(12)]
    public uint VirtualAddress;
    [FieldOffset(16)]
    public uint SizeOfRawData;
    [FieldOffset(20)]
    public uint PointerToRawData;
  }
}
