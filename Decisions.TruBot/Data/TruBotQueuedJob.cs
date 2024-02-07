using DecisionsFramework;
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

        private TruBotProcess RunningProcess { get; set;  }

        public TruBotQueuedJob(TruBotProcess runningProcess, string id)
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
        
        public static void StartThreadJob(TruBotProcess process)
        {
            if (process == null)
            {
                log.Error("TruBot process was not found.");
                return;
            }
            
            string queueName = $"TruBot-QueuedJob-{process.BotName}";
            if (!ThreadJobService.HasJobInQueue(queueName))
            {
                ThreadJobService.AddToQueue(DateTime.Now, 
                    new TruBotQueuedJob(process, queueName),
                    queueName);
            }
        }

        public void Initialize()
        {
            //retrieve list of trubot process entitiies
            
            //for each entity, do below
            //StartThreadJob();
        }
    }
}