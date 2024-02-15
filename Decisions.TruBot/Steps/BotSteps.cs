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
    [AutoRegisterMethodsOnClass(true, "Integration/TruBot/Bot")]
    [ShapeImageAndColorProvider(null, TruBotSettings.TRUBOT_IMAGES_PATH)]
    public class BotSteps
    {
        public TruBotResponse RunBot(int botId,
            [IgnoreMappingDefault, PropertyClassification(0, "Override Base URL", "Settings")] string? overrideBaseUrl)
        {
            if (botId == null)
            {
                throw new BusinessRuleException("botId cannot be null.");
            }

            string baseUrl = ModuleSettingsAccessor<TruBotSettings>.GetSettings().GetBaseBotUrl(overrideBaseUrl);
            string url = $"{baseUrl}/RunBot";

            try
            {
                TruBotAuthentication auth = TruBotAuthentication.GetTruBotAuthentication(overrideBaseUrl);
                
                BotIdRequest inputs = new BotIdRequest();
                inputs.BotId = botId;
                
                JsonContent content = JsonContent.Create(inputs);
                
                string result = TruBotRest.TruBotPost(url, auth, content);
                TruBotResponse response = TruBotResponse.JsonDeserialize(result);
                
                if (response.Status == 401)
                {
                    result = TruBotRest.TruBotPost(url, TruBotAuthentication.Login(overrideBaseUrl), content);
                    response = TruBotResponse.JsonDeserialize(result);
                }

                return response;
            }
            catch (Exception ex)
            {
                throw new BusinessRuleException("The request to TruBot was unsuccessful.", ex);
            }
        }

        public JobStatusResponse GetJobStatusByJobId(TruBotAuthentication authentication, int jobId,
            [IgnoreMappingDefault, PropertyClassification(0, "Override Base URL", "Settings")] string? overrideBaseUrl)
        {
            if (jobId == null)
            {
                throw new BusinessRuleException("jobId cannot be null.");
            }

            string baseUrl = ModuleSettingsAccessor<TruBotSettings>.GetSettings().GetBaseBotUrl(overrideBaseUrl);

            try
            {
                JobIdRequest inputs = new JobIdRequest();
                inputs.JobId = jobId;
                
                JsonContent content = JsonContent.Create(inputs);
                
                string result = TruBotRest.TruBotPost($"{baseUrl}/GetJobStatus", authentication, content);
                
                return JobStatusResponse.JsonDeserialize(result);
            }
            catch (Exception ex)
            {
                throw new BusinessRuleException("The request to TruBot was unsuccessful.", ex);
            }
        }
        
        public JobStatusResponse GetJobStatusByJobExecutionId(TruBotAuthentication authentication, string jobExecutionId,
            [IgnoreMappingDefault, PropertyClassification(0, "Override Base URL", "Settings")] string? overrideBaseUrl)
        {
            if (string.IsNullOrEmpty(jobExecutionId))
            {
                throw new BusinessRuleException("jobExecutionId cannot be null or empty.");
            }

            string baseUrl = ModuleSettingsAccessor<TruBotSettings>.GetSettings().GetBaseBotUrl(overrideBaseUrl);

            try
            {
                JobExecutionIdRequest inputs = new JobExecutionIdRequest();
                inputs.JobExecutionId = jobExecutionId;

                JsonContent content = JsonContent.Create(inputs);
                
                string result = TruBotRest.TruBotPost($"{baseUrl}/GetJobStatus", authentication, content);
                
                return JobStatusResponse.JsonDeserialize(result);
            }
            catch (Exception ex)
            {
                throw new BusinessRuleException("The request to TruBot was unsuccessful.", ex);
            }
        }
        
        public JobDetailsResponse GetJobDetails(TruBotAuthentication authentication, DateTime fromInitiationDateTime, DateTime toInitiationDateTime, int? botId,
            [IgnoreMappingDefault, PropertyClassification(0, "Override Base URL", "Settings")] string? overrideBaseUrl)
        {
            string baseUrl = ModuleSettingsAccessor<TruBotSettings>.GetSettings().GetBaseBotUrl(overrideBaseUrl);

            try
            {
                DateRangeBotRequest inputs = new DateRangeBotRequest();
                DateRangeRequest inputsNoId = new DateRangeRequest();
                
                if (botId != null)
                {
                    inputs.FromInitiationDateTime = fromInitiationDateTime;
                    inputs.ToInitiationDateTime = toInitiationDateTime;
                    inputs.BotId = (int)botId;
                }
                else
                {
                    inputsNoId.FromInitiationDateTime = fromInitiationDateTime;
                    inputsNoId.ToInitiationDateTime = toInitiationDateTime;
                }

                JsonContent content = (botId != null) ? JsonContent.Create(inputs) : JsonContent.Create(inputsNoId);
                
                string result = TruBotRest.TruBotPost($"{baseUrl}/GetJobDetails", authentication, content);
                
                return JobDetailsResponse.JsonDeserialize(result);
            }
            catch (Exception ex)
            {
                throw new BusinessRuleException("The request to TruBot was unsuccessful.", ex);
            }
        }
        
        public BotVariablesValuesResponse GetBotVariablesValues(TruBotAuthentication authentication, string jobExecutionId, int botId, int jobId,
            [IgnoreMappingDefault, PropertyClassification(0, "Override Base URL", "Settings")] string? overrideBaseUrl)
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
                BotVariableValuesRequest inputs = new BotVariableValuesRequest();
                inputs.JobExecutionId = jobExecutionId;
                inputs.BotId = botId;
                inputs.JobId = jobId;
                
                JsonContent content = JsonContent.Create(inputs);
                
                string result = TruBotRest.TruBotPost($"{baseUrl}", authentication, content);
                
                return BotVariablesValuesResponse.JsonDeserialize(result);
            }
            catch (Exception ex)
            {
                throw new BusinessRuleException("The request to TruBot was unsuccessful.", ex);
            }
        }
        
        public ProcessInformationResponse GetProcessInformationByName(TruBotAuthentication authentication, string processName,
            [IgnoreMappingDefault, PropertyClassification(0, "Override Base URL", "Settings")] string? overrideBaseUrl)
        {
            if (string.IsNullOrEmpty(processName))
            {
                throw new BusinessRuleException("processName cannot be null or empty.");
            }

            string baseUrl = ModuleSettingsAccessor<TruBotSettings>.GetSettings().GetProcessInformationUrl(overrideBaseUrl);

            try
            {
                ProcessNameRequest inputs = new ProcessNameRequest();
                inputs.ProcessName = processName;
                
                JsonContent content = JsonContent.Create(inputs);
                
                string result = TruBotRest.TruBotPost($"{baseUrl}", authentication, content);
                
                return ProcessInformationResponse.JsonDeserialize(result);
            }
            catch (Exception ex)
            {
                throw new BusinessRuleException("The request to TruBot was unsuccessful.", ex);
            }
        }
        
        public ProcessInformationResponse GetProcessInformationByBotId(TruBotAuthentication authentication, int botId,
            [IgnoreMappingDefault, PropertyClassification(0, "Override Base URL", "Settings")] string? overrideBaseUrl)
        {
            if (botId == null)
            {
                throw new BusinessRuleException("botId cannot be null.");
            }

            string baseUrl = ModuleSettingsAccessor<TruBotSettings>.GetSettings().GetProcessInformationUrl(overrideBaseUrl);

            try
            {
                BotIdRequest inputs = new BotIdRequest();
                inputs.BotId = botId;
                
                JsonContent content = JsonContent.Create(inputs);
                
                string result = TruBotRest.TruBotPost($"{baseUrl}", authentication, content);

                return ProcessInformationResponse.JsonDeserialize(result);
            }
            catch (Exception ex)
            {
                throw new BusinessRuleException("The request to TruBot was unsuccessful.", ex);
            }
        }
    }
}