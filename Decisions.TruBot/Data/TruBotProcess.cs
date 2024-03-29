using Decisions.TruBot.Api;
using DecisionsFramework.Data.ORMapper;
using DecisionsFramework.Design.Properties;
using DecisionsFramework.ServiceLayer;
using DecisionsFramework.Utilities;

namespace Decisions.TruBot.Data
{
    [ORMEntity("trubot_process")]
    public class TruBotProcess : AbstractFolderEntity
    {
        public TruBotProcess()
        {
            Id = IDUtility.GetNewIdString();
            Status = "Started";
        }
        
        [ORMPrimaryKeyField]
        [PropertyHidden]
        public string Id { get; set; }
        
        [ORMField]
        public string WorkflowName { get; set; }
        
        [ORMField]
        public int BotId { get; set; }
        
        [ORMField]
        public string? BotName { get; set; }
        
        [ORMField]
        public string Status { get; set; }
        
        [ORMField]
        public DateTime StartTime { get; set; }
        
        [ORMField]
        public string StepDuration { get; set; }
        
        [ORMField]
        public string WorkflowId { get; set; }
        
        public string FlowTrackingId { get; set; }

        public string StepTrackingId { get; set; }
        
        public string UsedUrl { get; set; }
        
        public string? JobExecutionId { get; set; }
        
        public int WaitTime { get; set; }

        public override void BeforeSave()
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = IDUtility.GetNewIdString();
            }
            base.BeforeSave();
        }

        static ORM<TruBotProcess> orm = new();

        internal static TruBotProcess GetTruBotProcess(string truBotProcessId)
        {
            return orm.Fetch(truBotProcessId);
        }
        
        internal static TruBotProcess GetTruBotProcessByBotId(string truBotId)
        {
            return orm.Fetch(new WhereCondition[]
            {
                new FieldWhereCondition("BotId", QueryMatchType.Equals, truBotId)
            }).FirstOrDefault()!;
        }

        internal static TruBotProcess[] GetRunningTruBotProcesses()
        {
            return orm.Fetch(new WhereCondition[]
            {
                new FieldWhereCondition("status", QueryMatchType.Equals, TruBotConstants.STATUS_STARTED),
                new FieldWhereCondition("status", QueryMatchType.Equals, TruBotConstants.STATUS_DEPLOYING),
                new FieldWhereCondition("status", QueryMatchType.Equals, TruBotConstants.STATUS_IN_PROGRESS)
            });
        }

        public static void StartProcess(TruBotProcess botProcess)
        {
            ORM<TruBotProcess> botProcessOrm = new ORM<TruBotProcess>();
            botProcessOrm.Store(botProcess);
            
            TruBotThreadJob.StartThreadJob(botProcess);
        }
        
        public static void CompleteProcess(TruBotProcess botProcess, JobStatusResponse statusResponse)
        {
            ORM<TruBotProcess> botProcessOrm = new ORM<TruBotProcess>();
            
            botProcess.StepDuration = (DateTime.Now - botProcess.StartTime).ToString();
            botProcess.Status = statusResponse.Status ?? "Error";
            
            botProcessOrm.Store(botProcess);
        }
    }
}