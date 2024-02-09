using DecisionsFramework.Data.DataTypes;
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
        }

        [ORMPrimaryKeyField]
        [PropertyHidden]
        public string Id { get; set; }
        
        [ORMField]
        public string WorkflowName { get; set; }
        
        [ORMField]
        public int BotId { get; set; }
        
        [ORMField]
        public string BotName { get; set; }
        
        [ORMField]
        public DateTime StartTime { get; set; }
        
        [ORMField]
        public string StepDuration { get; set; }

        public override void BeforeSave()
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = IDUtility.GetNewIdString();
            }
            base.BeforeSave();
        }

        static ORM<TruBotProcess> orm = new();

        public TruBotProcess(string workflowName, int botId, DateTime startTime)
        {
            Id = IDUtility.GetNewIdString();
            WorkflowName = workflowName;
            BotId = botId;
            StartTime = startTime;
        }

        internal static TruBotProcess GetTruBotProcess(string truBotProcessId)
        {
            return orm.Fetch(new WhereCondition[]
            {
                new FieldWhereCondition("id", QueryMatchType.Equals, truBotProcessId)
            }).FirstOrDefault();
        }
        
        internal static TruBotProcess GetTruBotProcessByBotId(string truBotId)
        {
            return orm.Fetch(new WhereCondition[]
            {
                new FieldWhereCondition("bot_id", QueryMatchType.Equals, truBotId)
            }).FirstOrDefault();
        }

        internal static TruBotProcess[] GetRunningTruBotProcesses()
        {
            return orm.Fetch(new WhereCondition[]
            {
                new FieldWhereCondition("Status", QueryMatchType.Equals, "Started")
            });
        }
    }
}