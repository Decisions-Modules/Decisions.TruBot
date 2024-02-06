using DecisionsFramework;
using DecisionsFramework.Utilities;

namespace Decisions.TruBot.Data
{
    public class TruBotQueuedJob : IThreadJob
    {
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
    }
}