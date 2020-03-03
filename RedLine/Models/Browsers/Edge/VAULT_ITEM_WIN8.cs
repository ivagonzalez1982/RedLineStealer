using System;

namespace RedLine.Models.Browsers.Edge
{
  public struct VAULT_ITEM_WIN8
  {
    public Guid SchemaId;
    public IntPtr pszCredentialFriendlyName;
    public IntPtr pResourceElement;
    public IntPtr pIdentityElement;
    public IntPtr pAuthenticatorElement;
    public IntPtr pPackageSid;
    public ulong LastModified;
    public uint dwFlags;
    public uint dwPropertiesCount;
    public IntPtr pPropertyElements;
  }
}
