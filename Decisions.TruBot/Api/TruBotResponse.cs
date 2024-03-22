using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using Newtonsoft.Json;

namespace Decisions.TruBot.Api
{
    [Writable]
    public class TruBotResponse
    {
        [WritableValue]
        [JsonProperty("status")]
        public int Status { get; set; }

        [WritableValue]
        [JsonProperty("message")]
        public string? Message { get; set; }
        
        [WritableValue]
        [JsonProperty("botId")]
        public int BotId { get; set; }

        [WritableValue]
        [JsonProperty("botName")]
        public string? BotName { get; set; }

        [WritableValue]
        [JsonProperty("botStationName")]
        public string? BotStationName { get; set; }
        
        [WritableValue]
        [JsonProperty("jobId")]
        public int JobId { get; set; }
        
        [WritableValue]
        [JsonProperty("jobStatus")]
        public string? JobStatus { get; set; }
        
        [WritableValue]
        [JsonProperty("jobExecutionId")]
        public string? JobExecutionId { get; set; }

        public static TruBotResponse JsonDeserialize(string json)
        {
            return JsonConvert.DeserializeObject<TruBotResponse>(json) ?? new TruBotResponse();
        }
    }
}