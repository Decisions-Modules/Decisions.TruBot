using Decisions.TruBot.Data;
using DecisionsFramework;
using DecisionsFramework.ServiceLayer;

namespace Decisions.TruBot
{
    public class TruBotInitializer : IInitializable
    {
        private static Log log = new Log("TruBot Initializer");
        
        public void Initialize()
        {
            TruBotProcess[] processes = TruBotProcess.GetRunningTruBotProcesses();

            if (processes.Length > 0)
            {
                foreach (TruBotProcess process in processes)
                {
                    TruBotThreadJob.StartThreadJob(process);
                }
            }
        }
    }
}