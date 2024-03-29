using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using Newtonsoft.Json;

namespace Decisions.TruBot.Api
{
    [Writable]
    public class JobDetailsData
    {
        [WritableValue]
        [JsonProperty("botId")]
        public int BotId { get; set; }
        
        [WritableValue]
        [JsonProperty("botName")]
        public string? BotName { get; set; }
        
        [WritableValue]
        [JsonProperty("jobId")]
        public int JobId { get; set; }
        
        [WritableValue]
        [JsonProperty("jobExecutionId")]
        public string? JobExecutionId { get; set; }
        
        [WritableValue]
        [JsonProperty("botStationId")]
        public int BotStationId { get; set; }
        
        [WritableValue]
        [JsonProperty("botStationName")]
        public string? BotStationName { get; set; }
        
        [WritableValue]
        [JsonProperty("status")]
        public string? Status { get; set; }
        
        [WritableValue]
        [JsonProperty("message")]
        public string? Message { get; set; }
        
        [WritableValue]
        [JsonProperty("percentageComplete")]
        public double? PercentageComplete { get; set; }
        
        [WritableValue]
        [JsonProperty("initiatedOn")]
        public DateTime? InitiatedOn { get; set; }
        
        [WritableValue]
        [JsonProperty("completedOn")]
        public DateTime? CompletedOn { get; set; }
    }

    [Writable]
    public class JobDetailsResponse
    {
        [WritableValue]
        [JsonProperty("code")]
        public int Code { get; set; }
        
        [WritableValue]
        [JsonProperty("data")]
        public JobDetailsData[]? Data { get; set; }
        
        public static JobDetailsResponse JsonDeserialize(string json)
        {
            return JsonConvert.DeserializeObject<JobDetailsResponse>(json) ?? new JobDetailsResponse();
        }
    }
}