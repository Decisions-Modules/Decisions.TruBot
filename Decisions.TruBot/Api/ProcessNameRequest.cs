using System.Runtime.Serialization;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using Newtonsoft.Json;

namespace Decisions.TruBot.Api
{
    [DataContract]
    [Writable]
    public class ProcessNameRequest
    {
        [WritableValue]
        [JsonProperty("ProcessName")]
        public string ProcessName { get; set; }
    }
}