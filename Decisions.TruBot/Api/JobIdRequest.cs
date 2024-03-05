using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using Newtonsoft.Json;

namespace Decisions.TruBot.Api
{
    [Writable]
    public class JobIdRequest
    {
        [WritableValue]
        [JsonProperty("JobId")]
        public int JobId { get; set; }
    }
}