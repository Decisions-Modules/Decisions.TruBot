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

            string baseUrl = ModuleSettingsAccessor<TruBotSettings>.GetSettings().GetBotUrl(overrideBaseUrl);
            string url = $"{baseUrl}/RunBot";
            
            TruBotAuthentication auth = new TruBotAuthentication
            {
                Token = ModuleSettingsAccessor<TruBotSettings>.GetSettings().Token,
                Sid = ModuleSettingsAccessor<TruBotSettings>.GetSettings().Sid
            };

            try
            {
                BotIdRequest inputs = new BotIdRequest();
                inputs.BotId = botId;
                
                JsonContent content = JsonContent.Create(inputs);
                
                string result = TruBotRest.TruBotPost(url, auth, content);
                TruBotResponse response = TruBotResponse.JsonDeserialize(result);

                return response;
            }
            catch (Exception ex)
            {
                throw new BusinessRuleException("The request to TruBot was unsuccessful.", ex);
            }
        }

        public JobStatusResponse GetJobStatusByJobId(int jobId,
            [IgnoreMappingDefault, PropertyClassification(0, "Override Base URL", "Settings")] string? overrideBaseUrl)
        {
            if (jobId == null)
            {
                throw new BusinessRuleException("jobId cannot be null.");
            }

            string baseUrl = ModuleSettingsAccessor<TruBotSettings>.GetSettings().GetBotUrl(overrideBaseUrl);
            
            TruBotAuthentication auth = new TruBotAuthentication
            {
                Token = ModuleSettingsAccessor<TruBotSettings>.GetSettings().Token,
                Sid = ModuleSettingsAccessor<TruBotSettings>.GetSettings().Sid
            };

            try
            {
                JobIdRequest inputs = new JobIdRequest();
                inputs.JobId = jobId;
                
                JsonContent content = JsonContent.Create(inputs);
                
                string result = TruBotRest.TruBotPost($"{baseUrl}/GetJobStatus", auth, content);
                
                return JobStatusResponse.JsonDeserialize(result);
            }
            catch (Exception ex)
            {
                throw new BusinessRuleException("The request to TruBot was unsuccessful.", ex);
            }
        }
        
        public JobStatusResponse GetJobStatusByJobExecutionId(string jobExecutionId,
            [IgnoreMappingDefault, PropertyClassification(0, "Override Base URL", "Settings")] string? overrideBaseUrl)
        {
            if (string.IsNullOrEmpty(jobExecutionId))
            {
                throw new BusinessRuleException("jobExecutionId cannot be null or empty.");
            }

            string baseUrl = ModuleSettingsAccessor<TruBotSettings>.GetSettings().GetBotUrl(overrideBaseUrl);
            
            TruBotAuthentication auth = new TruBotAuthentication
            {
                Token = ModuleSettingsAccessor<TruBotSettings>.GetSettings().Token,
                Sid = ModuleSettingsAccessor<TruBotSettings>.GetSettings().Sid
            };

            try
            {
                JobExecutionIdRequest inputs = new JobExecutionIdRequest();
                inputs.JobExecutionId = jobExecutionId;

                JsonContent content = JsonContent.Create(inputs);
                
                string result = TruBotRest.TruBotPost($"{baseUrl}/GetJobStatus", auth, content);
                
                return JobStatusResponse.JsonDeserialize(result);
            }
            catch (Exception ex)
            {
                throw new BusinessRuleException("The request to TruBot was unsuccessful.", ex);
            }
        }
        
        public JobDetailsResponse GetJobDetails(DateTime fromInitiationDateTime, DateTime toInitiationDateTime, int? botId,
            [IgnoreMappingDefault, PropertyClassification(0, "Override Base URL", "Settings")] string? overrideBaseUrl)
        {
            string baseUrl = ModuleSettingsAccessor<TruBotSettings>.GetSettings().GetBotUrl(overrideBaseUrl);
            
            TruBotAuthentication auth = new TruBotAuthentication
            {
                Token = ModuleSettingsAccessor<TruBotSettings>.GetSettings().Token,
                Sid = ModuleSettingsAccessor<TruBotSettings>.GetSettings().Sid
            };

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
                
                string result = TruBotRest.TruBotPost($"{baseUrl}/GetJobDetails", auth, content);
                
                return JobDetailsResponse.JsonDeserialize(result);
            }
            catch (Exception ex)
            {
                throw new BusinessRuleException("The request to TruBot was unsuccessful.", ex);
            }
        }
        
        public BotVariablesValuesResponse GetBotVariablesValues(string jobExecutionId, int botId, int jobId,
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
            
            TruBotAuthentication auth = new TruBotAuthentication
            {
                Token = ModuleSettingsAccessor<TruBotSettings>.GetSettings().Token,
                Sid = ModuleSettingsAccessor<TruBotSettings>.GetSettings().Sid
            };

            try
            {
                BotVariableValuesRequest inputs = new BotVariableValuesRequest();
                inputs.JobExecutionId = jobExecutionId;
                inputs.BotId = botId;
                inputs.JobId = jobId;
                
                JsonContent content = JsonContent.Create(inputs);
                
                string result = TruBotRest.TruBotPost($"{baseUrl}", auth, content);
                
                return BotVariablesValuesResponse.JsonDeserialize(result);
            }
            catch (Exception ex)
            {
                throw new BusinessRuleException("The request to TruBot was unsuccessful.", ex);
            }
        }
        
        public ProcessInformationResponse GetProcessInformationByName(string processName,
            [IgnoreMappingDefault, PropertyClassification(0, "Override Base URL", "Settings")] string? overrideBaseUrl)
        {
            if (string.IsNullOrEmpty(processName))
            {
                throw new BusinessRuleException("processName cannot be null or empty.");
            }

            string baseUrl = ModuleSettingsAccessor<TruBotSettings>.GetSettings().GetProcessInformationUrl(overrideBaseUrl);
            
            TruBotAuthentication auth = new TruBotAuthentication
            {
                Token = ModuleSettingsAccessor<TruBotSettings>.GetSettings().Token,
                Sid = ModuleSettingsAccessor<TruBotSettings>.GetSettings().Sid
            };

            try
            {
                ProcessNameRequest inputs = new ProcessNameRequest();
                inputs.ProcessName = processName;
                
                JsonContent content = JsonContent.Create(inputs);
                
                string result = TruBotRest.TruBotPost($"{baseUrl}", auth, content);
                
                return ProcessInformationResponse.JsonDeserialize(result);
            }
            catch (Exception ex)
            {
                throw new BusinessRuleException("The request to TruBot was unsuccessful.", ex);
            }
        }
        
        public ProcessInformationResponse GetProcessInformationByBotId(int botId,
            [IgnoreMappingDefault, PropertyClassification(0, "Override Base URL", "Settings")] string? overrideBaseUrl)
        {
            if (botId == null)
            {
                throw new BusinessRuleException("botId cannot be null.");
            }

            string baseUrl = ModuleSettingsAccessor<TruBotSettings>.GetSettings().GetProcessInformationUrl(overrideBaseUrl);
            
            TruBotAuthentication auth = new TruBotAuthentication
            {
                Token = ModuleSettingsAccessor<TruBotSettings>.GetSettings().Token,
                Sid = ModuleSettingsAccessor<TruBotSettings>.GetSettings().Sid
            };

            try
            {
                BotIdRequest inputs = new BotIdRequest();
                inputs.BotId = botId;
                
                JsonContent content = JsonContent.Create(inputs);
                
                string result = TruBotRest.TruBotPost($"{baseUrl}", auth, content);

                return ProcessInformationResponse.JsonDeserialize(result);
            }
            catch (Exception ex)
            {
                throw new BusinessRuleException("The request to TruBot was unsuccessful.", ex);
            }
        }
    }
}