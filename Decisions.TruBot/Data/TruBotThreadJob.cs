using System.Data;
using DecisionsFramework;
using DecisionsFramework.Data.ORMapper;
using DecisionsFramework.ServiceLayer;
using DecisionsFramework.Utilities;

namespace Decisions.TruBot.Data
{
    public class TruBotThreadJob : IThreadJob, IInitializable
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

        public TruBotThreadJob(TruBotProcess? runningProcess, string id)
        {
            RunningProcess = runningProcess;
            Id = id;
        }
        
        private TruBotProcess[] RunningProcesses { get; set;  }

        public TruBotThreadJob(TruBotProcess[] runningProcesses, string id)
        {
            RunningProcesses = runningProcesses;
            Id = id;
        }

        public void Run()
        {
            string queueName = $"TruBot-Queued-Job-{Id}";
            RunningProcesses = TruBotProcess.GetRunningTruBotProcesses();

            if (!ThreadJobService.HasJobInQueue(queueName))
            {
                ThreadJobService.AddToQueue(DateTime.Now.AddSeconds(5), this, queueName);
            }

            TruBotAuthentication auth = TruBotAuthentication.GetTruBotAuthentication(null);
        }
        
        public static void StartThreadJob(TruBotProcess? process)
        {
            if (process == null)
            {
                log.Error("TruBot process could not be started: not found.");
                return;
            }
            
            string queueName = $"TruBot-Queued-Job-{process.Id}";
            if (!ThreadJobService.HasJobInQueue(queueName))
            {
                log.Info($"Starting queue: {queueName}.");
                
                ThreadJobService.AddToQueue(DateTime.Now, 
                    new TruBotThreadJob(process, queueName),
                    queueName);
            }
            else
            {
                log.Error($"{queueName} already running.");
            }
        }
        
        public static void CompleteThreadJob(TruBotProcess process)
        {
            string queueName = $"TruBot-Queued-Job-{process.Id}";
            if (process.Id == null || !ThreadJobService.HasJobInQueue(queueName))
            {
                log.Error($"TruBot process could not be stopped: {queueName} not found).");
                return;
            }

            ThreadJobService.CurrentJob.State = JobState.Completed;
            ThreadJobService.RemoveFromQueue(process.Id);
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
                var executeStatement = new ExecuteStoredProcedureWithReturn("GetTruBotProcessesToStart");
                DynamicORM dorm = new DynamicORM();
                DataSet ds = dorm.RunQuery(executeStatement);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    List<string> ids = new List<string>();
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        var val = dr[0].ToString();
                        if (!string.IsNullOrEmpty(val))
                        {
                            ids.Add(val);
                        }
                    }
                    
                    foreach (string id in ids)
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