using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using Newtonsoft.Json;

namespace Decisions.TruBot.Api
{
    [Writable]
    public class JobStatusRequest
    {
        [WritableValue]
        [JsonProperty("JobId")]
        public int JobId { get; set; }
        
        [WritableValue]
        [JsonProperty("JobExecutionId")]
        public string JobExecutionId { get; set; }
    }
}