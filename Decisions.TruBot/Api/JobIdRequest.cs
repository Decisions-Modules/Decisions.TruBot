using System.Runtime.Serialization;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using Newtonsoft.Json;

namespace Decisions.TruBot.Api
{
    [DataContract]
    [Writable]
    public class JobIdRequest
    {
        [WritableValue]
        [JsonProperty("JobId")]
        public int JobId { get; set; }
    }
}