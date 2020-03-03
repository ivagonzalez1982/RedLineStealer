using System.Runtime.InteropServices;

namespace RedLine.Models.RunPE
{
  [StructLayout(LayoutKind.Explicit, Size = 20)]
  public struct IMAGE_FILE_HEADER
  {
    [FieldOffset(2)]
    public ushort NumberOfSections;
  }
}
