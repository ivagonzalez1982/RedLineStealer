using System.Runtime.InteropServices;

namespace RedLine.Models.RunPE
{
  [StructLayout(LayoutKind.Explicit, Size = 64)]
  public struct IMAGE_DOS_HEADER
  {
    [FieldOffset(0)]
    public ushort e_magic;
    [FieldOffset(60)]
    public uint e_lfanew;
  }
}
