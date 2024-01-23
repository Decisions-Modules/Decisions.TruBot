using DecisionsFramework;
using DecisionsFramework.Data.ORMapper;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.ServiceLayer.Utilities;

namespace Decisions.TruBot.Data
{
    [RegisterUser]
    //[AutoRegisterService("TruBot Invocation Service", typeof(ITruBotInvocationService))]
    public class TruBotInvocationService
    {
        private static readonly Log Log = new Log("External Invocation");
        
        public void Complete(AbstractUserContext userContext, string truBotEntityInvocationId)
        {
            ORM<TruBotInvocationEntity> truBotInvocationEntityOrm = new ORM<TruBotInvocationEntity>();
            TruBotInvocationEntity truBotInvocationEntity = truBotInvocationEntityOrm.Fetch(truBotEntityInvocationId);
            
            // Get the flow engine for the flow we'd like to complete
            FlowEngine engine = FlowEngine.GetEngine(truBotInvocationEntity.FlowTrackingId);

            // Call Done to tell the Engine the Step is complete
            engine.Done(truBotInvocationEntity.FlowTrackingId, truBotInvocationEntity.StepTrackingId, new ResultData("Done"));
            
            // Mark the Entity Invocation Completed
            truBotInvocationEntity.Status = "Completed";
            
            // Store the entity
            truBotInvocationEntityOrm.Store(truBotInvocationEntity);

            Log.Warn("Complete Operation Successfully Invoked");
        }

        public string GetStatus(AbstractUserContext userContext, string truBotEntityInvocationId)
        {
            ORM<TruBotInvocationEntity> truBotInvocationEntityOrm = new ORM<TruBotInvocationEntity>();
            TruBotInvocationEntity truBotInvocationEntity = truBotInvocationEntityOrm.Fetch(truBotEntityInvocationId);
            
            Log.Warn("Get Status Operation Successfully Invoked");

            return truBotInvocationEntity.Status;
        }
    }
}