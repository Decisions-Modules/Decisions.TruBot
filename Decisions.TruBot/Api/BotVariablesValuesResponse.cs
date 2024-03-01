using System.Runtime.Serialization;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using Newtonsoft.Json;

namespace Decisions.TruBot.Api
{
    [DataContract]
    [Writable]
    public class BotVariablesValuesData
    {
        [WritableValue]
        [JsonProperty("Name")]
        public string? Name { get; set; }
        
        [WritableValue]
        [JsonProperty("Value")]
        public string? Value { get; set; }

        [WritableValue]
        [JsonProperty("Type")]
        public string? Type { get; set; }
        
        [JsonProperty("Category")]
        public int Category { get; set; }
        
        [WritableValue]
        [JsonProperty("SecurityDetailsId")]
        public int SecurityDetailsId { get; set; }
        
        [WritableValue]
        [JsonProperty("IsCyberarkPassword")]
        public bool? IsCyberarkPassword { get; set; }
    }

    [DataContract]
    [Writable]
    public class BotVariablesValuesResponse
    {
        [WritableValue]
        [JsonProperty("Status")]
        public int Status { get; set; }
        
        [WritableValue]
        [JsonProperty("Message")]
        public string? Message { get; set; }
        
        [WritableValue]
        [JsonProperty("Data")]
        public BotVariablesValuesData[]? Data { get; set; }
        
        public static BotVariablesValuesResponse JsonDeserialize(string json)
        {
            return JsonConvert.DeserializeObject<BotVariablesValuesResponse>(json) ?? new BotVariablesValuesResponse();
        }
    }
}