using System.Runtime.Serialization;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using Newtonsoft.Json;

namespace Decisions.TruBot.Api
{
    [DataContract]
    [Writable]
    public class JobStatusData
    {
        [WritableValue]
        [JsonProperty("status")]
        public string? Status { get; set; }
        
        [WritableValue]
        [JsonProperty("reason")]
        public string? Reason { get; set; }
        
        [WritableValue]
        [JsonProperty("percentageComplete")]
        public double? PercentageComplete { get; set; }
        
        [WritableValue]
        [JsonProperty("processName")]
        public string? ProcessName { get; set; }
        
        [WritableValue]
        [JsonProperty("processVersion")]
        public string? ProcessVersion { get; set; }
        
        [WritableValue]
        [JsonProperty("botName")]
        public string? BotName { get; set; }
        
        [WritableValue]
        [JsonProperty("botId")]
        public int BotId { get; set; }
        
        [WritableValue]
        [JsonProperty("jobId")]
        public int JobId { get; set; }
        
        [WritableValue]
        [JsonProperty("jobExecutionId")]
        public string? JobExecutionId { get; set; }
    }

    [DataContract]
    [Writable]
    public class JobStatusResponse
    {
        [WritableValue]
        [JsonProperty("statusCode")]
        public int StatusCode { get; set; }
        
        [WritableValue]
        [JsonProperty("message")]
        public string? Message { get; set; }
        
        [WritableValue]
        [JsonProperty("data")]
        public JobStatusData? Data { get; set; }
        
        public static JobStatusResponse JsonDeserialize(string json)
        {
            return JsonConvert.DeserializeObject<JobStatusResponse>(json) ?? new JobStatusResponse();
        }
    }
}