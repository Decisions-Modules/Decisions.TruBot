using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using Newtonsoft.Json;

namespace Decisions.TruBot.Api
{
    [Writable]
    public class JobExecutionIdRequest
    {
        [WritableValue]
        [JsonProperty("JobExecutionId")]
        public string? JobExecutionId { get; set; }
    }
}