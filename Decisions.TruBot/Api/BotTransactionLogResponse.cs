using System.Runtime.Serialization;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using Newtonsoft.Json;

namespace Decisions.TruBot.Api
{
    [DataContract]
    [Writable]
    public class BotTransactionLogData
    {
        [WritableValue]
        [JsonProperty("id")]
        public int Id { get; set; }
        
        [WritableValue]
        [JsonProperty("applicationId")]
        public int ApplicationId { get; set; }
        
        [WritableValue]
        [JsonProperty("jobExecutionId")]
        public string? JobExecutionId { get; set; }
        
        [WritableValue]
        [JsonProperty("processName")]
        public string? ProcessName { get; set; }
        
        [WritableValue]
        [JsonProperty("botName")]
        public string? BotName { get; set; }
        
        [WritableValue]
        [JsonProperty("userName")]
        public string? UserName { get; set; }
        
        [WritableValue]
        [JsonProperty("machineIP")]
        public string? MachineIP { get; set; }
        
        [WritableValue]
        [JsonProperty("machineName")]
        public string? MachineName { get; set; }
        
        [WritableValue]
        [JsonProperty("transactionID1")]
        public string? TransactionID1 { get; set; }
        
        [WritableValue]
        [JsonProperty("transactionID2")]
        public string? TransactionID2 { get; set; }
        
        [WritableValue]
        [JsonProperty("transactionID3")]
        public string? TransactionID3 { get; set; }
        
        [WritableValue]
        [JsonProperty("transactionID4")]
        public string? TransactionID4 { get; set; }
        
        [WritableValue]
        [JsonProperty("transactionID5")]
        public string? TransactionID5 { get; set; }
        
        [WritableValue]
        [JsonProperty("activityName1")]
        public string? ActivityName1 { get; set; }
        
        [WritableValue]
        [JsonProperty("activityName2")]
        public string? ActivityName2 { get; set; }
        
        [WritableValue]
        [JsonProperty("activityName3")]
        public string? ActivityName3 { get; set; }
        
        [WritableValue]
        [JsonProperty("activityName4")]
        public string? ActivityName4 { get; set; }
        
        [WritableValue]
        [JsonProperty("activityName5")]
        public string? ActivityName5 { get; set; }
        
        [WritableValue]
        [JsonProperty("systemName")]
        public string? SystemName { get; set; }
        
        [WritableValue]
        [JsonProperty("date")]
        public DateTime? Date { get; set; }
        
        [WritableValue]
        [JsonProperty("startTime")]
        public string? StartTime { get; set; }
        
        [WritableValue]
        [JsonProperty("endTime")]
        public string? EndTime { get; set; }
        
        [WritableValue]
        [JsonProperty("status")]
        public string? Status { get; set; }

        [WritableValue]
        [JsonProperty("botId")]
        public int BotId { get; set; }
        
        [WritableValue]
        [JsonProperty("jobId")]
        public int JobId { get; set; }
        
        [WritableValue]
        [JsonProperty("remarks")]
        public string? Remarks { get; set; }
    }

    [DataContract]
    [Writable]
    public class BotTransactionLogResponse
    {
        [WritableValue]
        [JsonProperty("code")]
        public int Code { get; set; }
        
        [WritableValue]
        [JsonProperty("message")]
        public string? Message { get; set; }
        
        [WritableValue]
        [JsonProperty("data")]
        public JobStatusData? Data { get; set; }
        
        public static BotTransactionLogResponse JsonDeserialize(string json)
        {
            return JsonConvert.DeserializeObject<BotTransactionLogResponse>(json) ?? new BotTransactionLogResponse();
        }
    }
}