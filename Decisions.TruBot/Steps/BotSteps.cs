using Decisions.TruBot.Api;
using Decisions.TruBot.Data;
using DecisionsFramework;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.StepImplementations;
using DecisionsFramework.Design.Properties;
using DecisionsFramework.ServiceLayer;

namespace Decisions.TruBot.Steps
{
    [AutoRegisterMethodsOnClass(true, "Integration/TruBot/Bot")]
    [ShapeImageAndColorProvider(null, TruBotSettings.TRUBOT_IMAGES_PATH)]
    public class BotSteps
    {
        public TruBotResponse RunBot(TruBotAuthentication authentication, int botId,
            [PropertyClassification(0, "Override Base URL", "Settings")] string? overrideBaseUrl)
        {
            if (botId == null)
            {
                throw new BusinessRuleException("botId cannot be null.");
            }

            string baseUrl = ModuleSettingsAccessor<TruBotSettings>.GetSettings().GetBaseBotUrl(overrideBaseUrl);

            try
            {
                string result = TruBotRest.TruBotGet(
                    $"{baseUrl}/RunBot/{botId}",
                    authentication);
                
                return TruBotResponse.JsonDeserialize(result);
            }
            catch (Exception ex)
            {
                throw new BusinessRuleException("The request to TruBot was unsuccessful.", ex);
            }
        }
        
        public JobStatusResponse GetJobStatus(TruBotAuthentication authentication, string jobExecutionId, int jobId,
            [PropertyClassification(0, "Override Base URL", "Settings")] string? overrideBaseUrl)
        {
            if (string.IsNullOrEmpty(jobExecutionId))
            {
                throw new BusinessRuleException("jobExecutionId cannot be null or empty.");
            }
            
            if (jobId == null)
            {
                throw new BusinessRuleException("jobId cannot be null.");
            }

            string baseUrl = ModuleSettingsAccessor<TruBotSettings>.GetSettings().GetBaseBotUrl(overrideBaseUrl);

            try
            {
                string result = TruBotRest.TruBotGet(
                    $"{baseUrl}/GetJobStatus/JobExecutionId/{jobExecutionId}/JobId/{jobId}",
                    authentication);
                
                return JobStatusResponse.JsonDeserialize(result);
            }
            catch (Exception ex)
            {
                throw new BusinessRuleException("The request to TruBot was unsuccessful.", ex);
            }
        }
        
        public JobDetailsResponse GetJobDetails(TruBotAuthentication authentication, DateTime fromInitiationDateTime, DateTime toInitiationDateTime, int botId,
            [PropertyClassification(0, "Override Base URL", "Settings")] string? overrideBaseUrl)
        {
            if (botId == null)
            {
                throw new BusinessRuleException("botId cannot be null.");
            }

            string baseUrl = ModuleSettingsAccessor<TruBotSettings>.GetSettings().GetBaseBotUrl(overrideBaseUrl);

            try
            {
                string result = TruBotRest.TruBotGet(
                    $"{baseUrl}/GetJobDetails/FromInitiationDateTime/{fromInitiationDateTime}/ToInitiationDateTime/{toInitiationDateTime}/BotId/{botId}",
                    authentication);
                
                return JobDetailsResponse.JsonDeserialize(result);
            }
            catch (Exception ex)
            {
                throw new BusinessRuleException("The request to TruBot was unsuccessful.", ex);
            }
        }
        
        public BotVariablesValuesResponse GetBotVariablesValues(TruBotAuthentication authentication, string jobExecutionId, int botId, int jobId,
            [PropertyClassification(0, "Override Base URL", "Settings")] string? overrideBaseUrl)
        {
            if (string.IsNullOrEmpty(jobExecutionId))
            {
                throw new BusinessRuleException("jobExecutionId cannot be null or empty.");
            }
            
            if (botId == null)
            {
                throw new BusinessRuleException("botId cannot be null.");
            }
            
            if (jobId == null)
            {
                throw new BusinessRuleException("jobId cannot be null.");
            }

            string baseUrl = ModuleSettingsAccessor<TruBotSettings>.GetSettings().GetBotVariableValuesUrl(overrideBaseUrl);

            try
            {
                string result = TruBotRest.TruBotGet(
                    $"{baseUrl}/JobExecutionId/{jobExecutionId}/BotId/{botId}/JobId/{jobId}",
                    authentication);
                
                return BotVariablesValuesResponse.JsonDeserialize(result);
            }
            catch (Exception ex)
            {
                throw new BusinessRuleException("The request to TruBot was unsuccessful.", ex);
            }
        }
        
        public ProcessInformationResponse GetProcessInformationByName(TruBotAuthentication authentication, string processName,
            [PropertyClassification(0, "Override Base URL", "Settings")] string? overrideBaseUrl)
        {
            if (string.IsNullOrEmpty(processName))
            {
                throw new BusinessRuleException("processName cannot be null or empty.");
            }

            string baseUrl = ModuleSettingsAccessor<TruBotSettings>.GetSettings().GetProcessInformationUrl(overrideBaseUrl);

            try
            {
                string result = TruBotRest.TruBotGet(
                    $"{baseUrl}/ProcessName/{processName}",
                    authentication);
                
                return ProcessInformationResponse.JsonDeserialize(result);
            }
            catch (Exception ex)
            {
                throw new BusinessRuleException("The request to TruBot was unsuccessful.", ex);
            }
        }
        
        public ProcessInformationResponse GetProcessInformationByBotId(TruBotAuthentication authentication, int botId,
            [PropertyClassification(0, "Override Base URL", "Settings")] string? overrideBaseUrl)
        {
            if (botId == null)
            {
                throw new BusinessRuleException("botId cannot be null.");
            }

            string baseUrl = ModuleSettingsAccessor<TruBotSettings>.GetSettings().GetProcessInformationUrl(overrideBaseUrl);

            try
            {
                string result = TruBotRest.TruBotGet(
                    $"{baseUrl}/BotId/{botId}",
                    authentication);
                
                return ProcessInformationResponse.JsonDeserialize(result);
            }
            catch (Exception ex)
            {
                throw new BusinessRuleException("The request to TruBot was unsuccessful.", ex);
            }
        }
    }
}