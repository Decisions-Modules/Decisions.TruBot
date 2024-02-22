using System.Net.Http.Json;
using Decisions.TruBot.Api;
using DecisionsFramework;
using DecisionsFramework.Data.ORMapper;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.ServiceLayer;
using DecisionsFramework.ServiceLayer.Actions;
using DecisionsFramework.ServiceLayer.Actions.Common;
using DecisionsFramework.ServiceLayer.Services.Assignments;
using DecisionsFramework.ServiceLayer.Utilities;

namespace Decisions.TruBot.Data
{
    [Writable]
    public class TruBotAssignmentHandler : IAssignmentHandler
    {
        private static Log log = new Log("TruBot Assignments");
        
        private Assignment currentAssignment;

        [WritableValue]
        private TruBotProcess truBotProcess;

        public TruBotAssignmentHandler(TruBotProcess truBotProcess)
        {
            this.truBotProcess = truBotProcess;
        }

        public BaseActionType[] GetAssignmentActions(AbstractUserContext userContext, Assignment assignment)
        {
            if (truBotProcess == null)
                throw new Exception("TruBot process is not defined");

            currentAssignment = assignment;

            if (assignment.IsCurrent)
            {
                List<BaseActionType> actions = new List<BaseActionType>
                {
                    new DisplayMethodReturnAction(
                        "Ignore Assignment",
                        String.Format("Ignore {0}", truBotProcess.Id),
                        Ignore) {Order = 2}
                };

                try
                {
                    actions.Add(new DisplayMethodReturnAction(
                        "Update if Finished",
                        String.Format("Check status for {0} and close assignment if completed or failed", truBotProcess.Id),
                        RefreshStatus) { Order = 1, RefreshScope = ActionRefreshScope.OwningFolder });
                }
                catch
                {
                    log.Debug("TruBot process not found, so actions cannot be created.");
                }
                
                return actions.ToArray();
            }
            
            return new BaseActionType[0];
        }
        
        private string RefreshStatus(AbstractUserContext usercontext, string botId)
        {
            try
            {
                TruBotAuthentication.Login(null, null, null);
                
                TruBotAuthentication auth = new TruBotAuthentication
                {
                    Token = ModuleSettingsAccessor<TruBotSettings>.GetSettings().Token,
                    Sid = ModuleSettingsAccessor<TruBotSettings>.GetSettings().Sid
                };

                JobExecutionIdRequest statusRequest = new JobExecutionIdRequest
                {
                    JobExecutionId = truBotProcess.JobExecutionId
                };

                JobStatusResponse statusResponse = JobStatusResponse.JsonDeserialize(
                    TruBotRest.TruBotPost($"{truBotProcess.UsedUrl}/GetJobStatus", auth,
                        JsonContent.Create(statusRequest)));
                

                if (statusResponse.Status != "Deploying" && statusResponse.Status != "In Progress")
                {
                    ORM<TruBotProcess> botProcessOrm = new ORM<TruBotProcess>();
            
                    truBotProcess.Status = "Completed";
            
                    TimeSpan stepDuration = DateTime.Now - truBotProcess.StartTime;
                    truBotProcess.StepDuration = $"{stepDuration.Duration().Hours}:{stepDuration.Duration().Minutes}:{stepDuration.Duration().Seconds}";
            
                    botProcessOrm.Store(truBotProcess);
                    
                    CloseAssignment(usercontext);
                }

                return String.Format("Status for TruBot process '{0}' has been updated", truBotProcess.Id);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return "Resend Failed with error: " + ex.Message;
            }
        }
        
        private string Ignore(AbstractUserContext usercontext, string entityid)
        {
            CloseAssignment(usercontext);
            return String.Format("The assignment for '{0}' has been closed", truBotProcess.Id);
        }
        
        private void CloseAssignment(AbstractUserContext usercontext)
        {
            currentAssignment.Completed = true;
            currentAssignment.Hidden = true;
            AssignmentService.Instance.Save(usercontext, currentAssignment);
        }
        
        public string GetEntityShortName(Assignment assignment)
        {
            return "TruBot Assignment";
        }
    }
}