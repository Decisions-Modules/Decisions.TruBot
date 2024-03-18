using System.Net.Http.Json;
using Decisions.TruBot.Api;
using Decisions.TruBot.Data;
using DecisionsFramework;
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
    [ShapeImageAndColorProvider(null, TruBotConstants.TRUBOT_IMAGES_PATH)]
    public class WaitRunBotStep : IAsyncStep, IDataConsumer
    {
        private const string PATH_DONE = "Done";
        private const string TRUBOT_RESPONSE = "TruBot Status Response";
        
        private const string BOT_ID = "BotId";
        private const string WAIT_TIME = "Minutes Between Status Checks";
        
        [WritableValue]
        private string? overrideBaseUrl;

        [PropertyClassification(0, "Override Base URL", "Override Settings")]
        public string? OverrideBaseUrl
        {
            get => overrideBaseUrl;
            set => overrideBaseUrl = value;
        }
        
        public void Start(StepStartData data)
        {
            TruBotSettings settings = ModuleSettingsAccessor<TruBotSettings>.GetSettings();
            
            int botId = (int)data.Data[BOT_ID];
            int waitTime = ((int)data.Data[WAIT_TIME] >= 1) ? (int)data.Data[WAIT_TIME] : 1;

            string baseUrl = OverrideBaseUrl ?? settings.GetBotUrl();
            string url = $"{baseUrl}/RunBot";

            try
            {
                BotIdRequest inputs = new BotIdRequest();
                inputs.BotId = botId;
                
                JsonContent content = JsonContent.Create(inputs);
                
                DateTime startTime = DateTime.Now;
                string result = TruBotRest.TruBotPost(url, content);
                TruBotResponse response = TruBotResponse.JsonDeserialize(result);

                TruBotRecordedBot.Create(response.BotId, response.BotName, startTime);

                TruBotProcess.StartProcess(new TruBotProcess
                {
                    WorkflowName = FlowEngine.CurrentFlow.Name,
                    BotId = botId,
                    BotName = response.BotName,
                    StartTime = startTime,
                    FlowTrackingId = data.FlowTrackingID,
                    StepTrackingId = data.StepTrackingID,
                    UsedUrl = baseUrl,
                    JobExecutionId = response.JobExecutionId,
                    WaitTime = waitTime
                });
            }
            catch (Exception ex)
            {
                throw new BusinessRuleException("The request to TruBot was unsuccessful.", ex);
            }
        }
        
        public OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                return new[]
                {
                    new OutcomeScenarioData(PATH_DONE, new DataDescription(typeof(JobStatusResponse), TRUBOT_RESPONSE)
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
                    new DataDescription(typeof(int), WAIT_TIME)
                });
            
                return input.ToArray();
            }
        }
    }
}