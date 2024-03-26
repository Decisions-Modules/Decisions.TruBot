using System.Net.Http.Json;
using Decisions.TruBot.Api;
using Decisions.TruBot.Behaviors;
using Decisions.TruBot.Data;
using DecisionsFramework;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.StepImplementations;
using DecisionsFramework.Design.Properties;
using DecisionsFramework.ServiceLayer;

namespace Decisions.TruBot.Steps
{
    [AutoRegisterMethodsOnClass(true, "Integration/TruBot/Bot")]
    [ShapeImageAndColorProvider(null, TruBotConstants.TRUBOT_IMAGES_PATH)]
    public class BotSteps
    {
        public TruBotResponse RunBot(int botId,
            [IgnoreMappingDefault, PropertyClassification(0, "Override Base URL", "Settings")] string? overrideBaseUrl)
        {
            TruBotSettings settings = ModuleSettingsAccessor<TruBotSettings>.GetSettings();
            string baseUrl = overrideBaseUrl ?? settings.GetBotUrl();
            string url = $"{baseUrl}/RunBot";
            
            try
            {
                BotIdRequest inputs = new BotIdRequest();
                inputs.BotId = botId;
                
                JsonContent content = JsonContent.Create(inputs);
                
                DateTime startTime = DateTime.Now;
                string result = TruBotRest.TruBotPost(url, content);
                TruBotResponse response = TruBotResponse.JsonDeserialize(result);

                string projectFolder = GetTruBotProjectFolderFromFlow();
                TruBotRecordedBot.Create(response.BotId, response.BotName, startTime, projectFolder);

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
            TruBotSettings settings = ModuleSettingsAccessor<TruBotSettings>.GetSettings();
            string baseUrl = overrideBaseUrl ?? settings.GetBotUrl();
            string url = $"{baseUrl}/GetJobStatus";
            
            try
            {
                JobIdRequest inputs = new JobIdRequest();
                inputs.JobId = jobId;
                
                JsonContent content = JsonContent.Create(inputs);
                
                string result = TruBotRest.TruBotPost(url, content);
                
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
            TruBotSettings settings = ModuleSettingsAccessor<TruBotSettings>.GetSettings();
            string baseUrl = overrideBaseUrl ?? settings.GetBotUrl();
            string url = $"{baseUrl}/GetJobStatus";
            
            try
            {
                JobExecutionIdRequest inputs = new JobExecutionIdRequest();
                inputs.JobExecutionId = jobExecutionId;

                JsonContent content = JsonContent.Create(inputs);
                
                string result = TruBotRest.TruBotPost(url, content);
                
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
            TruBotSettings settings = ModuleSettingsAccessor<TruBotSettings>.GetSettings();
            string baseUrl = overrideBaseUrl ?? settings.GetBotUrl();
            string url = $"{baseUrl}/GetJobDetails";
            
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
                
                string result = TruBotRest.TruBotPost(url, content);
                
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
            TruBotSettings settings = ModuleSettingsAccessor<TruBotSettings>.GetSettings();
            string url = overrideBaseUrl ?? settings.GetBotVariableValuesUrl();
            
            try
            {
                BotVariableValuesRequest inputs = new BotVariableValuesRequest();
                inputs.JobExecutionId = jobExecutionId;
                inputs.BotId = botId;
                inputs.JobId = jobId;
                
                JsonContent content = JsonContent.Create(inputs);
                
                string result = TruBotRest.TruBotPost(url, content);
                
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
            TruBotSettings settings = ModuleSettingsAccessor<TruBotSettings>.GetSettings();
            string url = overrideBaseUrl ?? settings.GetProcessInformationUrl();
            
            try
            {
                ProcessNameRequest inputs = new ProcessNameRequest
                {
                    ProcessName = processName
                };

                JsonContent content = JsonContent.Create(inputs);
                
                string result = TruBotRest.TruBotPost(url, content);
                
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
            TruBotSettings settings = ModuleSettingsAccessor<TruBotSettings>.GetSettings();
            string url = overrideBaseUrl ?? settings.GetProcessInformationUrl();
            
            try
            {
                BotIdRequest inputs = new BotIdRequest();
                inputs.BotId = botId;
                
                JsonContent content = JsonContent.Create(inputs);
                
                string result = TruBotRest.TruBotPost(url, content);

                return ProcessInformationResponse.JsonDeserialize(result);
            }
            catch (Exception ex)
            {
                throw new BusinessRuleException("The request to TruBot was unsuccessful.", ex);
            }
        }

        public static string GetTruBotProjectFolderFromFlow() => 
            TruBotInterfacesFolderBehavior.GetTruBotFolderId(
                FlowEngine.CurrentFlow.GetElementRegistration()
                    .GetProjectId());
    }
}