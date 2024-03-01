using System.Runtime.Serialization;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using Newtonsoft.Json;

namespace Decisions.TruBot.Api
{
    [DataContract]
    [Writable]
    public class JobStatusResponse
    {
        [WritableValue]
        [JsonProperty("status")]
        public string? Status { get; set; }
        
        [WritableValue]
        [JsonProperty("message")]
        public string? Message { get; set; }
        
        [WritableValue]
        [JsonProperty("jobExecutionId")]
        public string? JobExecutionId { get; set; }
        
        [WritableValue]
        [JsonProperty("botId")]
        public int BotId { get; set; }
        
        [WritableValue]
        [JsonProperty("jobId")]
        public int JobId { get; set; }
        
        [WritableValue]
        [JsonProperty("percentageComplete")]
        public double? PercentageComplete { get; set; }
        
        public static JobStatusResponse JsonDeserialize(string json)
        {
            return JsonConvert.DeserializeObject<JobStatusResponse>(json) ?? new JobStatusResponse();
        }
    }
}