using System.Runtime.Serialization;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using Newtonsoft.Json;

namespace Decisions.TruBot.Api
{
    [DataContract]
    [Writable]
    public class DateRangeRequest
    {
        [WritableValue]
        [JsonProperty("FromInitiationDateTime")]
        public DateTime FromInitiationDateTime { get; set; }
        
        [WritableValue]
        [JsonProperty("ToInitiationDateTime")]
        public DateTime ToInitiationDateTime { get; set; }
    }
}