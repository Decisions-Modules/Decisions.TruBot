using System.ServiceModel;
using DecisionsFramework.ServiceLayer.Utilities;

namespace Decisions.TruBot.Data
{
    [ServiceContract]
    public interface ITruBotInvocationService
    {
        [OperationContract]
        void Complete(AbstractUserContext userContext, string truBotEntityInvocationId);

        [OperationContract]
        string? GetStatus(AbstractUserContext userContext, string truBotEntityInvocationId);
    }
}