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

            DateTime startTime = DateTime.Now;
            
            ORM<TruBotProcess> botProcessOrm = new ORM<TruBotProcess>();
            TruBotProcess botProcess = new TruBotProcess(FlowEngine.CurrentFlow.Name, botId, startTime);
            TruBotThreadJob.StartThreadJob(botProcess);

            botProcessOrm.Store(botProcess);

            string baseUrl = ModuleSettingsAccessor<TruBotSettings>.GetSettings().GetBaseBotUrl(OverrideBaseUrl);
            string runUrl = $"{baseUrl}/RunBot";

            Dictionary<string, object> resultData = new Dictionary<string, object>();
            
            string? statusResponse;
            TruBotResponse response = new TruBotResponse();
            try
            {
                TruBotAuthentication auth = TruBotAuthentication.GetTruBotAuthentication(overrideBaseUrl);
                
                BotIdRequest inputs = new BotIdRequest();
                inputs.BotId = botId;
                
                JsonContent content = JsonContent.Create(inputs);
                
                string result = TruBotRest.TruBotPost(runUrl, auth, content);
                response = TruBotResponse.JsonDeserialize(result);

                if (response.Status == 401)
                {
                    result = TruBotRest.TruBotPost(runUrl, TruBotAuthentication.Login(overrideBaseUrl), content);
                    response = TruBotResponse.JsonDeserialize(result);
                }
                
                JobExecutionIdRequest statusRequestInput = new JobExecutionIdRequest();
                statusRequestInput.JobExecutionId = response.JobExecutionId;
                
                JsonContent statusContent = JsonContent.Create(statusRequestInput);

                statusResponse = response.JobStatus;
                while ((statusResponse == "Deploying" || statusResponse == "In Progress"))
                {
                    Thread.Sleep(15000);
                    
                    statusResponse = JobStatusResponse.JsonDeserialize(
                        TruBotRest.TruBotPost($"{baseUrl}/GetJobStatus", auth, statusContent)).Status;
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

            TimeSpan stepDuration = DateTime.Now - botProcess.StartTime;
            botProcess.StepDuration = $"{stepDuration.Duration().Hours}:{stepDuration.Duration().Minutes}:{stepDuration.Duration().Seconds}";
            
            botProcessOrm.Store(botProcess);
            TruBotThreadJob.CompleteThreadJob(botProcess);
            
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
                    new DataDescription(typeof(int), BOT_ID)
                });
            
                return input.ToArray();
            }
        }
    }
}