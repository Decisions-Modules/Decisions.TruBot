using System.Text;
using Decisions.TruBot.Api;
using Decisions.TruBot.Data;
using DecisionsFramework;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using DecisionsFramework.Design.Flow.StepImplementations;
using DecisionsFramework.Design.Properties;
using DecisionsFramework.Design.Properties.Attributes;
using DecisionsFramework.ServiceLayer;
using DecisionsFramework.Utilities.Data;

namespace Decisions.TruBot.Steps
{
    [Writable]
    [AutoRegisterStep("TruBot Login", "Integration/TruBot")]
    [ShapeImageAndColorProvider(null, TruBotSettings.TRUBOT_IMAGES_PATH)]
    public class TruBotLogin : ISyncStep, IDataConsumer
    {
        private const string PATH_DONE = "Done";
        private const string AUTHENTICATION_RESPONSE = "TruBot Authentication Response";
        private const string TRUBOT_CREDENTIALS = "TruBot Credentials";
        
        [WritableValue]
        private string overrideBaseUrl;

        [PropertyClassification(0, "Override Base URL", "Override Settings")]
        public string OverrideBaseUrl
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
        private string username;

        [BooleanPropertyHidden("OverrideCredentials", false)]
        [PropertyClassification(2, "Override Username", "Override Settings")]
        public string Username
        {
            get => username;
            set => username = value;
        }
        
        [WritableValue]
        private string password;
        
        [PasswordText]
        [BooleanPropertyHidden("OverrideCredentials", false)]
        [PropertyClassification(3, "Override Password", "Override Settings")]
        public string Password
        {
            get => password;
            set => password = value;
        }
        
        public ResultData Run(StepStartData data)
        {
            HttpClient client = HttpClients.GetHttpClient(HttpClientAuthType.Normal);
            string baseUrl = ModuleSettingsAccessor<TruBotSettings>.GetSettings().GetBaseAccountUrl(OverrideBaseUrl);
            string? credentials = null;
            
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/Login");

            if (!OverrideCredentials)
            {
                string settingsUsername = ModuleSettingsAccessor<TruBotSettings>.GetSettings().Username;
                string settingsPassword = ModuleSettingsAccessor<TruBotSettings>.GetSettings().Password;

                if (string.IsNullOrEmpty(settingsUsername) || string.IsNullOrEmpty(settingsPassword))
                {
                    throw new BusinessRuleException("Configure credentials in settings or override on step.");
                }
                
                credentials = $"{settingsUsername}:{settingsPassword}";
            }
            else if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
            {
                credentials = $"{Username}:{Password}";
            }

            byte[] credentialsBytes = Encoding.UTF8.GetBytes(credentials);
            string base64Credentials = Convert.ToBase64String(credentialsBytes);

            // Set the Authorization header
            request.Headers.Add("Authorization", $"Basic {base64Credentials}");

            AuthenticationResponse authenticationResponse;
            TruBotAuthentication truBotCredentials;
            try
            {
                HttpResponseMessage response = client.Send(request);
                response.EnsureSuccessStatusCode();

                Task<string> resultTask = response.Content.ReadAsStringAsync();
                resultTask.Wait();
                
                authenticationResponse = AuthenticationResponse.JsonDeserialize(resultTask.Result);

                truBotCredentials = new TruBotAuthentication()
                {
                    sid = authenticationResponse.Sid,
                    token = authenticationResponse.Token
                };
            }
            catch (Exception ex)
            {
                throw new BusinessRuleException("The request to TruBot was unsuccessful.", ex);
            }
            
            Dictionary<string, object> resultData = new Dictionary<string, object>();
            resultData.Add(AUTHENTICATION_RESPONSE, authenticationResponse);
            resultData.Add(TRUBOT_CREDENTIALS, truBotCredentials);

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
                    }, new DataDescription(typeof(TruBotAuthentication), TRUBOT_CREDENTIALS)
                    {
                        DisplayName = TRUBOT_CREDENTIALS
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