using System.Runtime.Serialization;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using Newtonsoft.Json;

namespace Decisions.TruBot.Api
{
    [DataContract]
    [Writable]
    public class AuthenticationResponse
    {
        [WritableValue]
        [JsonProperty("code")]
        public string? Code { get; set; }

        [WritableValue]
        [JsonProperty("token")]
        public string? Token { get; set; }

        [WritableValue]
        [JsonProperty("message")]
        public string? Message { get; set; }

        [WritableValue]
        [JsonProperty("sid")]
        public string? Sid { get; set; }

        public static AuthenticationResponse JsonDeserialize(string json)
        {
            return JsonConvert.DeserializeObject<AuthenticationResponse>(json) ?? new AuthenticationResponse();
        }
    }
}