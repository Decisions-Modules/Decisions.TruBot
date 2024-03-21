using DecisionsFramework.Data.ORMapper;
using DecisionsFramework.Design.Properties;
using DecisionsFramework.ServiceLayer;
using DecisionsFramework.Utilities;

namespace Decisions.TruBot.Data
{
    [ORMEntity("trubot_recorded_bots")]
    public class TruBotRecordedBot : AbstractFolderEntity
    {
        public TruBotRecordedBot()
        {
        }

        [ORMPrimaryKeyField]
        [PropertyHidden]
        public string Id { get; set; }
        
        [ORMField]
        public int BotId { get; set; }
        
        [ORMField]
        public string? BotName { get; set; }
        
        [ORMField]
        public DateTime InitializedOn { get; set; }
        
        [ORMField]
        public DateTime LastRunOn { get; set; }
        
        [ORMField]
        public string ProjectId { get; set; }
        
        static ORM<TruBotRecordedBot> orm = new();

        public TruBotRecordedBot(int botId)
        {
            TruBotRecordedBot? bot = GetTruBotRecordByBotId(botId);

            if (bot != null)
            {
                bot = GetTruBotRecordByBotId(botId);

                Id = bot.Id;
                BotId = botId;
                BotName = bot.BotName;
                InitializedOn = bot.InitializedOn;
                LastRunOn = bot.LastRunOn;

                return;
            }
            
            Id = IDUtility.GetNewIdString();
            BotId = botId;
            InitializedOn = DateTime.Now;
        }

        internal static TruBotRecordedBot GetTruBotRecord(string truBotProcessId)
        {
            return orm.Fetch(truBotProcessId);
        }
        
        internal static TruBotRecordedBot? GetTruBotRecordByBotId(int truBotId)
        {
            return orm.Fetch(new WhereCondition[]
            {
                new FieldWhereCondition("bot_id", QueryMatchType.Equals, truBotId)
            }).FirstOrDefault();
        }

        public static TruBotRecordedBot Create(int botId, string? botName, DateTime startTime)
        {
            ORM<TruBotRecordedBot> recordedBotOrm = new ORM<TruBotRecordedBot>();
            TruBotRecordedBot recordedBot = new TruBotRecordedBot(botId);
            recordedBot.BotName = botName;
            recordedBot.LastRunOn = startTime;
                
            recordedBotOrm.Store(recordedBot);

            return recordedBot;
        }
    }
}