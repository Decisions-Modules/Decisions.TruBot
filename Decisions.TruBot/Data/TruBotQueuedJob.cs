using System.Data;
using DecisionsFramework;
using DecisionsFramework.Data.ORMapper;
using DecisionsFramework.ServiceLayer;
using DecisionsFramework.Utilities;

namespace Decisions.TruBot.Data
{
    public class TruBotQueuedJob : IThreadJob, IInitializable
    {
        private static Log log = new Log("TruBot Queued Job");
        
        private string id;
        
        public string Id
        {
            get
            {
                if (string.IsNullOrEmpty(id))
                {
                    id = IDUtility.GetNewIdString();
                }

                return id;
            }
            set { id = value; }
        }
        
        private static readonly Log Log = new("TruBot Queued Job");

        private TruBotProcess? RunningProcess { get; set;  }

        public TruBotQueuedJob(TruBotProcess? runningProcess, string id)
        {
            RunningProcess = runningProcess;
            Id = id;
        }
        
        private TruBotProcess[] RunningProcesses { get; set;  }

        public TruBotQueuedJob(TruBotProcess[] runningProcesses, string id)
        {
            RunningProcesses = runningProcesses;
            Id = id;
        }

        public void Run()
        {
            Log.Info("TruBot Queued Job running.");
            RunningProcesses = TruBotProcess.GetRunningTruBotProcesses();

            if (RunningProcesses.Length == 0)
            {
                Log.Warn("No running TruBot processes available.");
            }
                
            ThreadJobService.AddToQueue(DateTime.Now.AddSeconds(5), this, $"TruBot-Queue-{Id}");
        }
        
        public static void StartThreadJob(TruBotProcess? process)
        {
            if (process == null)
            {
                log.Error("TruBot process was not found.");
                return;
            }
            
            string queueName = $"TruBot-QueuedJob-Bot#{process.BotId}";
            if (!ThreadJobService.HasJobInQueue(queueName))
            {
                ThreadJobService.AddToQueue(DateTime.Now, 
                    new TruBotQueuedJob(process, queueName),
                    queueName);
            }
        }
        
        internal static TruBotProcess? GetProcessById(string processId)
        {
            if (String.IsNullOrEmpty(processId))
                throw new ArgumentNullException("processId");

            return AbstractEntity.GetEntityById(processId) as TruBotProcess;
        }

        public void Initialize()
        {
            try
            {
                // Get Message Ids to pull
                var executeStatement = new ExecuteStoredProcedureWithReturn("GetTruBotProcessesToStart");
                DynamicORM dorm = new DynamicORM();
                DataSet ds = dorm.RunQuery(executeStatement);

                // Pick which messages to pull
                if (ds.Tables[0].Rows.Count > 0)
                {
                    var ids = new List<string>();
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        var val = dr[0].ToString();
                        if (!string.IsNullOrEmpty(val))
                        {
                            ids.Add(val);
                        }
                    }
                    
                    foreach (var id in ids)
                    {
                        StartThreadJob(GetProcessById(id));
                    }
                }
            }
            catch (Exception ex)
            {
                log.Debug($"Error occured while retrieving queued TruBot processes. ({ex.Message})");
            }
        }
    }
}