using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using Newtonsoft.Json;

namespace Decisions.TruBot.Api
{
    [Writable]
    public class BotVariableValuesRequest
    {
        [WritableValue]
        [JsonProperty("JobExecutionId")]
        public string JobExecutionId { get; set; }
        
        [WritableValue]
        [JsonProperty("BotId")]
        public int BotId { get; set; }
        
        [WritableValue]
        [JsonProperty("JobId")]
        public int JobId { get; set; }
    }
}