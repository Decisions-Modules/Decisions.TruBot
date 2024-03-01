using System.Runtime.Serialization;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using Newtonsoft.Json;

namespace Decisions.TruBot.Api
{
    [DataContract]
    [Writable]
    public class ProcessInformationData
    {
        [WritableValue]
        [JsonProperty("processName")]
        public string? ProcessName { get; set; }
        
        [WritableValue]
        [JsonProperty("processVersion")]
        public string? ProcessVersion { get; set; }

        [WritableValue]
        [JsonProperty("status")]
        public string? Status { get; set; }
    }

    [DataContract]
    [Writable]
    public class ProcessInformationResponse
    {
        [WritableValue]
        [JsonProperty("status")]
        public int Status { get; set; }
        
        [WritableValue]
        [JsonProperty("message")]
        public string? Message { get; set; }
        
        [WritableValue]
        [JsonProperty("data")]
        public ProcessInformationData[]? Data { get; set; }
        
        public static ProcessInformationResponse JsonDeserialize(string json)
        {
            return JsonConvert.DeserializeObject<ProcessInformationResponse>(json) ?? new ProcessInformationResponse();
        }
    }
}