using DecisionsFramework;
using DecisionsFramework.Data.ORMapper;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.ServiceLayer.Utilities;

namespace Decisions.TruBot.Data
{
    [RegisterUser]
    [AutoRegisterService("TruBotInvocationService", typeof(ITruBotInvocationService))]
    public class TruBotInvocationService : ITruBotInvocationService
    {
        private static readonly Log Log = new("TruBot Invocation");
        
        public void Complete(AbstractUserContext userContext, string truBotEntityInvocationId)
        {
            ORM<TruBotInvocationEntity> truBotInvocationEntityOrm = new ORM<TruBotInvocationEntity>();
            TruBotInvocationEntity truBotInvocationEntity = truBotInvocationEntityOrm.Fetch(truBotEntityInvocationId);
            
            FlowEngine engine = FlowEngine.GetEngine(truBotInvocationEntity.FlowTrackingId);
            engine.Done(truBotInvocationEntity.FlowTrackingId, truBotInvocationEntity.StepTrackingId, new ResultData("Done"));
            
            truBotInvocationEntity.Status = "Completed";
            truBotInvocationEntityOrm.Store(truBotInvocationEntity);

            Log.Warn($"Complete Operation Successfully Invoked for {truBotEntityInvocationId}");
        }

        public string GetStatus(AbstractUserContext userContext, string truBotEntityInvocationId)
        {
            ORM<TruBotInvocationEntity> truBotInvocationEntityOrm = new ORM<TruBotInvocationEntity>();
            TruBotInvocationEntity truBotInvocationEntity = truBotInvocationEntityOrm.Fetch(truBotEntityInvocationId);
            
            Log.Warn($"Get Status Operation Successfully Invoked for {truBotEntityInvocationId}");

            return truBotInvocationEntity.Status;
        }
    }
}