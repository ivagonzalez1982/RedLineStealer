using RedLine.Logic.Extensions;
using RedLine.Logic.Json;
using RedLine.Models;
using System.IO;
using System.Net;
using System.Text;

namespace RedLine.Logic.Helpers
{
  public static class GeoHelper
  {
    public static GeoInfo Get()
    {
      try
      {
        string str = string.Empty;
        try
        {
          using (WebClient webClient = new WebClient())
          {
            webClient.Headers["User-Agent"] = "Mozilla/4.0 (Compatible; Windows NT 5.1; MSIE 6.0) (compatible; MSIE 6.0; Windows NT 5.1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
            try
            {
              str = Encoding.UTF8.GetString(webClient.DownloadData("http://checkip.amazonaws.com/")).Trim();
            }
            catch
            {
            }
          }
        }
        catch
        {
        }
        if (string.IsNullOrEmpty(str))
        {
          try
          {
            str = new WebClient().DownloadString("https://ipinfo.io/ip").Replace("\n", "");
          }
          catch
          {
          }
        }
        if (string.IsNullOrEmpty(str))
        {
          try
          {
            str = new WebClient().DownloadString("https://api.ipify.org").Replace("\n", "");
          }
          catch
          {
          }
        }
        if (string.IsNullOrEmpty(str))
        {
          try
          {
            str = new WebClient().DownloadString("https://icanhazip.com").Replace("\n", "");
          }
          catch
          {
          }
        }
        if (string.IsNullOrEmpty(str))
        {
          try
          {
            str = new WebClient().DownloadString("https://wtfismyip.com/text").Replace("\n", "");
          }
          catch
          {
          }
        }
        if (string.IsNullOrEmpty(str))
        {
          try
          {
            str = new WebClient().DownloadString("http://bot.whatismyipaddress.com/").Replace("\n", "");
          }
          catch
          {
          }
        }
        if (string.IsNullOrEmpty(str))
        {
          try
          {
            str = new StreamReader(WebRequest.Create("http://checkip.dyndns.org").GetResponse().GetResponseStream()).ReadToEnd().Trim().Split(':')[1].Substring(1).Split('<')[0];
          }
          catch
          {
          }
        }
        JsonValue jsonValue = new WebClient().DownloadString("http://www.geoplugin.net/json.gp?ip=" + str).FromJSON();
        return new GeoInfo()
        {
          Location = jsonValue["geoplugin_city"].ToString(false) == "null" ? jsonValue["geoplugin_latitude"].ToString(false) + ", " + jsonValue["geoplugin_longitude"].ToString(false) : jsonValue["geoplugin_city"].ToString(false),
          Country = jsonValue["geoplugin_countryCode"].ToString(false),
          IP = jsonValue["geoplugin_request"].ToString(false)
        };
      }
      catch
      {
        return new GeoInfo();
      }
    }
  }
}
