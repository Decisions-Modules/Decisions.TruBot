using System.Runtime.Serialization;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;

namespace Decisions.TruBot.Data
{
    [DataContract]
    [Writable]
    public class TruBotProcess
    {
        [WritableValue]
        private string workflowName;
        
        [WritableValue]
        private string botName;
        
        [WritableValue]
        private DateTime startTime;
        
        [WritableValue]
        private TimeOnly stepDuration;
    }
}