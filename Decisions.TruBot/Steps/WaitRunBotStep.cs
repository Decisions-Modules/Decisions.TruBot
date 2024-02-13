using System.Net.Http.Json;
using Decisions.TruBot.Api;
using Decisions.TruBot.Data;
using DecisionsFramework;
using DecisionsFramework.Data.ORMapper;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using DecisionsFramework.Design.Flow.StepImplementations;
using DecisionsFramework.Design.Properties;
using DecisionsFramework.ServiceLayer;
using DecisionsFramework.Utilities;

namespace Decisions.TruBot.Steps
{
    [Writable]
    [AutoRegisterStep("Run Bot and Wait", "Integration/TruBot/Bot")]
    [ShapeImageAndColorProvider(null, TruBotSettings.TRUBOT_IMAGES_PATH)]
    public class WaitRunBotStep : ISyncStep, IDataConsumer
    {
        private const string PATH_DONE = "Done";
        private const string TRUBOT_RESPONSE = "TruBot Response";
        
        private const string BOT_ID = "BotId";
        private const string AUTHENTICATION = "Authentication";
        
        [WritableValue]
        private string overrideBaseUrl;

        [PropertyClassification(0, "Override Base URL", "Override Settings")]
        public string OverrideBaseUrl
        {
            get => overrideBaseUrl;
            set => overrideBaseUrl = value;
        }

        public ResultData Run(StepStartData data)
        {
            int botId = data.Data[BOT_ID] as int? ?? 0;
            TruBotAuthentication? authentication = data.Data[AUTHENTICATION] as TruBotAuthentication;

            if (authentication == null)
            {
                throw new BusinessRuleException("Authentication cannot be null.");
            }
            
            DateTime startTime = DateTime.Now;
            
            ORM<TruBotProcess> botProcessOrm = new ORM<TruBotProcess>();
            TruBotProcess botProcess = new TruBotProcess(FlowEngine.CurrentFlow.Name, botId, startTime);
            TruBotQueuedJob.StartThreadJob(botProcess);

            botProcessOrm.Store(botProcess);

            /*ORM<TruBotInvocationEntity> botEntityOrm = new ORM<TruBotInvocationEntity>();
            
            TruBotInvocationEntity botEntity = new TruBotInvocationEntity();
            botEntity.Id = IDUtility.GetNewIdString();
            botEntity.Status = "Started";
            botEntity.FlowTrackingId = data.FlowTrackingID;
            botEntity.StepTrackingId = data.StepTrackingID;
            
            botEntityOrm.Store(botEntity);*/

            string baseUrl = ModuleSettingsAccessor<TruBotSettings>.GetSettings().GetBaseBotUrl(OverrideBaseUrl);

            Dictionary<string, object> resultData = new Dictionary<string, object>();
            
            string? statusResponse = "Deploying";
            TruBotResponse response = new TruBotResponse();
            try
            {
                BotIdRequest inputs = new BotIdRequest();
                inputs.BotId = botId;
                
                JsonContent content = JsonContent.Create(inputs);
                
                string result = TruBotRest.TruBotPost($"{baseUrl}/RunBot", authentication, content);
                
                response = TruBotResponse.JsonDeserialize(result);
                
                JobExecutionIdRequest statusRequestInput = new JobExecutionIdRequest();
                statusRequestInput.JobExecutionId = response.JobExecutionId;
                
                JsonContent statusContent = JsonContent.Create(statusRequestInput);

                statusResponse = response.JobStatus;
                while ((statusResponse == "Deploying" || statusResponse == "In Progress"))
                {
                    Thread.Sleep(15000);
                    
                    statusResponse = JobStatusResponse.JsonDeserialize(
                        TruBotRest.TruBotPost($"{baseUrl}/GetJobStatus", authentication, statusContent)).Status;
                }
            }
            catch (Exception ex)
            {
                throw new BusinessRuleException("The request to TruBot was unsuccessful.", ex);
            }
            
            TruBotResponse truBotResponse = new TruBotResponse
            {
                Status = response.Status,
                Message = response.Message,
                BotId = response.BotId,
                BotName = response.BotName,
                BotStationName = response.BotStationName,
                JobId = response.JobId,
                JobStatus = statusResponse ?? response.JobStatus,
                JobExecutionId = response.JobExecutionId
            };
                
            resultData.Add(TRUBOT_RESPONSE, truBotResponse);
            
            botProcess.BotName = truBotResponse.BotName;
            botProcess.Status = "Completed";
            botProcess.StepDuration = $"{(DateTime.Now - botProcess.StartTime).Duration().Seconds}s";
            
            botProcessOrm.Store(botProcess);
            TruBotQueuedJob.StopThreadJob(botProcess);
            
            /*botEntity.Status = truBotResponse.JobStatus;
            
            botEntityOrm.Store(botEntity);*/

            return new ResultData(PATH_DONE, resultData);
        }
        
        public OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                return new[]
                {
                    new OutcomeScenarioData(PATH_DONE, new DataDescription(typeof(TruBotResponse), TRUBOT_RESPONSE)
                    {
                        DisplayName = TRUBOT_RESPONSE
                    })
                };
            }
        }

        public DataDescription[] InputData
        {
            get
            {
                List<DataDescription> input = new List<DataDescription>();
                input.AddRange(new[]
                {
                    new DataDescription(typeof(int), BOT_ID),
                    new DataDescription(typeof(TruBotAuthentication), AUTHENTICATION)
                });
            
                return input.ToArray();
            }
        }
    }
}