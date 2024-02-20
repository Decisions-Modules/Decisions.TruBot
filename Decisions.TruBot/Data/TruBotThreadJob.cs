using System.Data;
using System.Net.Http.Json;
using Decisions.TruBot.Api;
using DecisionsFramework;
using DecisionsFramework.Data.ORMapper;
using DecisionsFramework.Design.Flow;
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
            TruBotAuthentication auth = new TruBotAuthentication
            {
                Token = ModuleSettingsAccessor<TruBotSettings>.GetSettings().Token,
                Sid = ModuleSettingsAccessor<TruBotSettings>.GetSettings().Sid
            };

            JobExecutionIdRequest statusRequest = new JobExecutionIdRequest
            {
                JobExecutionId = RunningProcess.JobExecutionId
            };

            JobStatusResponse statusResponse = JobStatusResponse.JsonDeserialize(
                TruBotRest.TruBotPost($"{RunningProcess.UsedUrl}/GetJobStatus", auth,
                    JsonContent.Create(statusRequest)));

            log.Warn($"{Id} status: {statusResponse.Status}");

            if ((statusResponse.Status != "Deploying" && statusResponse.Status != "In Progress"))
            {
                CompleteThreadJob(RunningProcess, statusResponse);
            }
            else
            {
                ThreadJobService.AddToQueue(DateTime.Now.AddSeconds(15), this, Id);
            }
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
                log.Warn($"Starting queue: {queueName}.");
                
                ThreadJobService.AddToQueue(DateTime.Now, 
                    new TruBotThreadJob(process, queueName),
                    queueName);
            }
            else
            {
                log.Error($"{queueName} already running.");
            }
        }
        
        public static void CompleteThreadJob(TruBotProcess process, JobStatusResponse statusResponse)
        {
            string queueName = $"TruBot-Queued-Job-{process.Id}";
            if (process.Id == null || !ThreadJobService.HasJobInQueue(queueName))
            {
                log.Error($"TruBot process could not be stopped: {queueName} not found).");
                return;
            }

            ORM<TruBotProcess> botProcessOrm = new ORM<TruBotProcess>();
            
            process.Status = "Completed";
            
            TimeSpan stepDuration = DateTime.Now - process.StartTime;
            process.StepDuration = $"{stepDuration.Duration().Hours}:{stepDuration.Duration().Minutes}:{stepDuration.Duration().Seconds}";
            
            botProcessOrm.Store(process);
            
            ResultData resultData = new ResultData();
            resultData.Add("TruBot Status Response", statusResponse);
            
            ThreadJobService.CurrentJob.State = JobState.Completed;
            ThreadJobService.RemoveFromQueue(process.Id);
            log.Warn($"TruBot process {queueName} is complete.");
            
            FlowEngine flowEngine = FlowEngine.GetEngine(process.FlowTrackingId);
            flowEngine.Done(process.FlowTrackingId, process.StepTrackingId, new ResultData("Done", resultData));
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
                        string? val = dr[0].ToString();
                        if (!string.IsNullOrEmpty(val))
                        {
                            ids.Add(val);
                        }
                    }
                    
                    foreach (string processId in ids)
                    {
                        StartThreadJob(GetProcessById(processId));
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