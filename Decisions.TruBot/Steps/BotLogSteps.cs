using System.Net.Http.Json;
using Decisions.TruBot.Api;
using Decisions.TruBot.Data;
using DecisionsFramework;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.StepImplementations;
using DecisionsFramework.Design.Properties;
using DecisionsFramework.ServiceLayer;

namespace Decisions.TruBot.Steps
{
    [AutoRegisterMethodsOnClass(true, "Integration/TruBot/BotLog")]
    [ShapeImageAndColorProvider(null, TruBotConstants.TRUBOT_IMAGES_PATH)]
    public class BotLogSteps
    {
        private readonly TruBotSettings Settings = ModuleSettingsAccessor<TruBotSettings>.GetSettings();
        
        public BotTransactionLogResponse GetBotTransactionLog(string jobExecutionId,
            [IgnoreMappingDefault, PropertyClassification(0, "Override Base URL", "Settings")] string? overrideBaseUrl)
        {
            string baseUrl = overrideBaseUrl ?? Settings.GetBotLogUrl();
            string url = $"{baseUrl}/GetBotTransactionLogs";

            try
            {
                JobExecutionIdRequest inputs = new JobExecutionIdRequest();
                inputs.JobExecutionId = jobExecutionId;

                JsonContent content = JsonContent.Create(inputs);
                
                string result = TruBotRest.TruBotPost(url, content);
                
                return BotTransactionLogResponse.JsonDeserialize(result);
            }
            catch (Exception ex)
            {
                throw new BusinessRuleException("The request to TruBot was unsuccessful.", ex);
            }
        }
        
        public void DownloadBotTransactionLog(string jobExecutionId, string destinationDirectory,
            [IgnoreMappingDefault, PropertyClassification(0, "Override Base URL", "Settings")] string? overrideBaseUrl)
        {
            string baseUrl = overrideBaseUrl ?? Settings.GetBotLogUrl();
            string url = $"{baseUrl}/DownloadBotTransactionLog";

            try
            {
                JobExecutionIdRequest inputs = new JobExecutionIdRequest();
                inputs.JobExecutionId = jobExecutionId;

                JsonContent content = JsonContent.Create(inputs);

                TruBotRest.TruBotDownload(url, destinationDirectory, jobExecutionId, content);
            }
            catch (Exception ex)
            {
                throw new BusinessRuleException("The request to TruBot was unsuccessful.", ex);
            }
        }
    }
}