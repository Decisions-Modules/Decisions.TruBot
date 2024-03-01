using DecisionsFramework;
using DecisionsFramework.ServiceLayer;

namespace Decisions.TruBot.Data
{
    public class TruBotInitializer : IInitializable
    {
        private static Log log = new Log("TruBot Initializer");
        
        public void Initialize()
        {
            TruBotAssignmentHelper.InitializeAssignmentComponents();
            
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