using System.Net.Http.Json;
using Decisions.TruBot.Api;
using DecisionsFramework;
using DecisionsFramework.Design.Flow;

namespace Decisions.TruBot.Data
{
    public class TruBotThreadJob : IThreadJob
    {
        private static Log log = new Log("TruBot Queued Job");
        
        public string Id { get; set; }

        private TruBotProcess? RunningProcess { get; set;  }

        public TruBotThreadJob(TruBotProcess? runningProcess, string id)
        {
            RunningProcess = runningProcess;
            Id = id;
        }
        
        public void Run()
        {
            JobExecutionIdRequest statusRequest = new JobExecutionIdRequest
            {
                JobExecutionId = RunningProcess.JobExecutionId
            };

            JobStatusResponse statusResponse = JobStatusResponse.JsonDeserialize(
                TruBotRest.TruBotPost($"{RunningProcess.UsedUrl}/GetJobStatus",
                    JsonContent.Create(statusRequest)));

            log.Debug($"{Id} status: {statusResponse.Status}");

            if ((statusResponse.Status != TruBotConstants.STATUS_DEPLOYING && statusResponse.Status != TruBotConstants.STATUS_IN_PROGRESS))
            {
                CompleteThreadJob(RunningProcess, statusResponse);
            }
            else
            {
                ThreadJobService.AddToQueue(DateTime.Now.AddMinutes(RunningProcess.WaitTime), this, Id);
            }
        }

        private static string SetQueueName(string processId)
        {
            return $"TruBot-Queued-Job-{processId}";
        }
        
        public static void StartThreadJob(TruBotProcess? process)
        {
            if (process == null)
            {
                log.Error("TruBot process could not be started: not found.");
                return;
            }
            
            string queueName = SetQueueName(process.Id);
            if (!ThreadJobService.HasJobInQueue(queueName))
            {
                log.Info($"Starting queue: {queueName}.");
                
                ThreadJobService.AddToQueue(DateTime.Now,
                    new TruBotThreadJob(process, queueName),
                    queueName);
            }
        }
        
        public static void CompleteThreadJob(TruBotProcess process, JobStatusResponse statusResponse)
        {
            string queueName = SetQueueName(process.Id);
            
            ResultData resultData = new ResultData();
            resultData.Add("TruBot Status Response", statusResponse);
            
            ThreadJobService.CurrentJob.State = JobState.Completed;
            ThreadJobService.RemoveFromQueue(process.Id);
            log.Info($"TruBot process {queueName} is complete.");
            
            FlowEngine flowEngine = FlowEngine.GetEngine(process.FlowTrackingId);
            flowEngine.Done(process.FlowTrackingId, process.StepTrackingId, new ResultData("Done", resultData));
        }
    }
}