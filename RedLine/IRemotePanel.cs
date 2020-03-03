using RedLine.Models;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace RedLine
{
  [ServiceContract]
  public interface IRemotePanel
  {
    [OperationContract]
    Task<ClientSettings> GetSettings();

    [OperationContract]
    Task SendClientInfo(UserLog user);

    [OperationContract]
    Task<IList<RemoteTask>> GetTasks(UserLog user);

    [OperationContract]
    Task CompleteTask(UserLog user, int taskId);
  }
}
