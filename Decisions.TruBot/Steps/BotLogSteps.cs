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
    [ShapeImageAndColorProvider(null, TruBotSettings.TRUBOT_IMAGES_PATH)]
    public class BotLogSteps
    {
        public BotTransactionLogResponse GetBotTransactionLog(TruBotAuthentication authentication, string jobExecutionId,
            [IgnoreMappingDefault, PropertyClassification(0, "Override Base URL", "Settings")] string? overrideBaseUrl)
        {
            if (string.IsNullOrEmpty(jobExecutionId))
            {
                throw new BusinessRuleException("jobExecutionId cannot be null or empty.");
            }

            string baseUrl = ModuleSettingsAccessor<TruBotSettings>.GetSettings().GetBaseBotLogUrl(overrideBaseUrl);

            try
            {
                JobExecutionIdRequest inputs = new JobExecutionIdRequest();
                inputs.JobExecutionId = jobExecutionId;

                JsonContent content = JsonContent.Create(inputs);
                
                string result = TruBotRest.TruBotPost($"{baseUrl}/GetBotTransactionLogs", authentication, content);
                
                return BotTransactionLogResponse.JsonDeserialize(result);
            }
            catch (Exception ex)
            {
                throw new BusinessRuleException("The request to TruBot was unsuccessful.", ex);
            }
        }
        
        public void DownloadBotTransactionLog(
            TruBotAuthentication authentication, string jobExecutionId, string destinationDirectory,
            [IgnoreMappingDefault, PropertyClassification(0, "Override Base URL", "Settings")] string? overrideBaseUrl)
        {
            if (string.IsNullOrEmpty(jobExecutionId))
            {
                throw new BusinessRuleException("jobExecutionId cannot be null or empty.");
            }

            string baseUrl = ModuleSettingsAccessor<TruBotSettings>.GetSettings().GetBaseBotLogUrl(overrideBaseUrl);

            try
            {
                JobExecutionIdRequest inputs = new JobExecutionIdRequest();
                inputs.JobExecutionId = jobExecutionId;

                JsonContent content = JsonContent.Create(inputs);

                TruBotRest.TruBotDownload($"{baseUrl}/DownloadBotTransactionLog", destinationDirectory, jobExecutionId, authentication, content);
            }
            catch (Exception ex)
            {
                throw new BusinessRuleException("The request to TruBot was unsuccessful.", ex);
            }
        }
    }
}