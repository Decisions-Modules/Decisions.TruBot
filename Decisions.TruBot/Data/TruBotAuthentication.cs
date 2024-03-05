using System.Runtime.Serialization;
using System.Text;
using Decisions.TruBot.Api;
using DecisionsFramework;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.ServiceLayer;
using DecisionsFramework.Utilities.Data;

namespace Decisions.TruBot.Data
{
    [DataContract]
    [Writable]
    public class TruBotAuthentication
    {
        [DataMember]
        [WritableValue]
        public string Token { get; set; }
        
        [DataMember]
        [WritableValue]
        public string Sid { get; set; }

        public void SetHeaders(HttpRequestMessage request)
        {
            request.Headers.Add("sid", Sid);
            request.Headers.Add("token", Token);
        }

        private static TruBotSettings Settings = ModuleSettingsAccessor<TruBotSettings>.GetSettings();
        
        public static AuthenticationResponse Login(string? overrideBaseUrl, string? username, string? password)
        {
            if (string.IsNullOrEmpty(username))
            {
                username = Settings.Username;
            }

            if (string.IsNullOrEmpty(password))
            {
                password = Settings.Password;
            }

            HttpClient client = HttpClients.GetHttpClient(HttpClientAuthType.Normal);
            string baseUrl = overrideBaseUrl ?? Settings.GetAccountUrl();
            string url = $"{baseUrl}/Login";

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
            
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                throw new BusinessRuleException("Credentials must be configured in TruBot settings.");
            }
            
            string credentials = $"{username}:{password}";

            byte[] credentialsBytes = Encoding.UTF8.GetBytes(credentials);
            string base64Credentials = Convert.ToBase64String(credentialsBytes);

            // Set the Authorization header
            request.Headers.Add("Authorization", $"Basic {base64Credentials}");

            AuthenticationResponse authenticationResponse;
            try
            {
                HttpResponseMessage response = client.Send(request);
                response.EnsureSuccessStatusCode();

                Task<string> resultTask = response.Content.ReadAsStringAsync();
                resultTask.Wait();
                
                authenticationResponse = AuthenticationResponse.JsonDeserialize(resultTask.Result);
            }
            catch (Exception ex)
            {
                throw new BusinessRuleException("The login request to TruBot was unsuccessful.", ex);
            }
            
            Settings.Token = authenticationResponse.Token;
            Settings.Sid = authenticationResponse.Sid;
            
            ModuleSettingsAccessor<TruBotSettings>.SaveSettings(Settings);

            return authenticationResponse;
        }
    }
}