using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using Newtonsoft.Json;

namespace Decisions.TruBot.Api
{
    [Writable]
    public class BotIdRequest
    {
        [WritableValue]
        [JsonProperty("BotId")]
        public int BotId { get; set; }
    }
}