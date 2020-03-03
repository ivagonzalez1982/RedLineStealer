using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;

namespace RedLine
{
  public static class Service<T>
  {
    public static string RemoteIP = string.Empty;
    private static readonly BasicHttpBinding binding;

    static Service()
    {
      BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
      basicHttpBinding.Name = "BindingName";
      basicHttpBinding.MaxBufferSize = int.MaxValue;
      basicHttpBinding.MaxReceivedMessageSize = (long) int.MaxValue;
      basicHttpBinding.MaxBufferPoolSize = (long) int.MaxValue;
      basicHttpBinding.CloseTimeout = TimeSpan.FromMinutes(30.0);
      basicHttpBinding.OpenTimeout = TimeSpan.FromMinutes(30.0);
      basicHttpBinding.ReceiveTimeout = TimeSpan.FromMinutes(30.0);
      basicHttpBinding.SendTimeout = TimeSpan.FromMinutes(30.0);
      basicHttpBinding.TransferMode = TransferMode.Buffered;
      basicHttpBinding.UseDefaultWebProxy = false;
      basicHttpBinding.ProxyAddress = (Uri) null;
      basicHttpBinding.ReaderQuotas = new XmlDictionaryReaderQuotas()
      {
        MaxDepth = 2000000,
        MaxArrayLength = int.MaxValue,
        MaxBytesPerRead = int.MaxValue,
        MaxNameTableCharCount = int.MaxValue,
        MaxStringContentLength = int.MaxValue
      };
      basicHttpBinding.Security = new BasicHttpSecurity()
      {
        Mode = BasicHttpSecurityMode.None
      };
      Service<T>.binding = basicHttpBinding;
      ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(Service<T>.AcceptAllCertifications);
    }

    private static bool AcceptAllCertifications(
      object sender,
      X509Certificate certification,
      X509Chain chain,
      SslPolicyErrors sslPolicyErrors)
    {
      return true;
    }

    public static void Use(Action<T> codeBlock)
    {
      using (new WebClient())
      {
        IClientChannel channel = (IClientChannel) (object) new ChannelFactory<T>((Binding) Service<T>.binding).CreateChannel(new EndpointAddress("http://" + Service<T>.RemoteIP + ":6677/IRemotePanel"));
        bool flag = false;
        try
        {
          codeBlock((T) channel);
          channel.Close();
          flag = true;
        }
        finally
        {
          if (!flag)
            channel.Abort();
        }
      }
    }
  }
}
