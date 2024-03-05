using Decisions.TruBot.Api;
using Decisions.TruBot.Data;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using DecisionsFramework.Design.Flow.StepImplementations;
using DecisionsFramework.Design.Properties;
using DecisionsFramework.Design.Properties.Attributes;

namespace Decisions.TruBot.Steps
{
    [Writable]
    [AutoRegisterStep("TruBot Login", "Integration/TruBot")]
    [ShapeImageAndColorProvider(null, TruBotConstants.TRUBOT_IMAGES_PATH)]
    public class TruBotLogin : ISyncStep, IDataConsumer
    {
        private const string PATH_DONE = "Done";
        private const string AUTHENTICATION_RESPONSE = "TruBot Authentication Response";
        
        [WritableValue]
        private string? overrideBaseUrl;

        [PropertyClassification(0, "Override Base URL", "Override Settings")]
        public string? OverrideBaseUrl
        {
            get => overrideBaseUrl;
            set => overrideBaseUrl = value;
        }
        
        [WritableValue]
        private bool overrideCredentials;
        
        [PropertyClassification(1, "Override Credentials", "Override Settings")]
        public bool OverrideCredentials
        {
            get => overrideCredentials;
            set => overrideCredentials = value;
        }
        
        [WritableValue]
        private string? username;

        [BooleanPropertyHidden("OverrideCredentials", false)]
        [PropertyClassification(2, "Override Username", "Override Settings")]
        public string? Username
        {
            get => username;
            set => username = value;
        }
        
        [WritableValue]
        private string? password;
        
        [PasswordText]
        [BooleanPropertyHidden("OverrideCredentials", false)]
        [PropertyClassification(3, "Override Password", "Override Settings")]
        public string? Password
        {
            get => password;
            set => password = value;
        }
        
        public ResultData Run(StepStartData data)
        {
            AuthenticationResponse authenticationResponse = (OverrideCredentials)
                ? TruBotAuthentication.Login(OverrideBaseUrl, Username, Password)
                : TruBotAuthentication.Login(OverrideBaseUrl, null, null);

            Dictionary<string, object> resultData = new Dictionary<string, object>();
            resultData.Add(AUTHENTICATION_RESPONSE, authenticationResponse);

            return new ResultData(PATH_DONE, resultData);
        }
        
        public OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                return new[]
                {
                    new OutcomeScenarioData(PATH_DONE, new DataDescription(typeof(AuthenticationResponse), AUTHENTICATION_RESPONSE)
                    {
                        DisplayName = AUTHENTICATION_RESPONSE
                    })
                };
            }
        }

        public DataDescription[] InputData
        {
            get;
        }
    }
}