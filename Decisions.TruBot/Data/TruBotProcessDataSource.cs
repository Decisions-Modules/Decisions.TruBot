using System.Data;
using DecisionsFramework.Data.ORMapper;
using DecisionsFramework.Design.Report;
using DecisionsFramework.ServiceLayer.Services.ContextData;

namespace Decisions.TruBot.Data
{
    [AutoRegisterReportElement("TruBot Process Data", "TruBot")]
    public class TruBotProcessDataSource : AbstractCustomDataSource, IFolderAwareFilter
    {
        public string FolderId { get; set; }

        public string TableName { get; }

        public TruBotProcessDataSource()
        {
            TableName = "trubot_process_data";
        }
        
        private readonly string ProcedureName = "GetTruBotProcess";
        private readonly string InterfaceParameterName = "id";

        public override bool Applies(ReportDefinition definition)
        {
            return true;
        }

        public override ReportFieldData[] ReportFields
        {
            get
            {
                return new[]
                {
                    new ReportFieldData(TableName, "id", "Id", typeof(string)),
                    new ReportFieldData(TableName, "workflow_name", "WorkflowName", typeof(string)),
                    new ReportFieldData(TableName, "bot_id", "BotId", typeof(string)),
                    new ReportFieldData(TableName, "bot_name", "BotName", typeof(string)),
                    new ReportFieldData(TableName, "status", "Status", typeof(string)),
                    new ReportFieldData(TableName, "start_time", "StartTime", typeof(DateTime)),
                    new ReportFieldData(TableName, "step_duration", "StepDuration", typeof(TimeSpan))
                };
            }
        }

        public override DataTable GetDataForPage(DataTable currentTable, IReportFilter[] filters, int? limitCount, int pageIndex, DataPair[] sortingInfo, Func<DataRow, bool> rowHandler)
        {
            ExecuteStoredProcedureWithReturn executeStatement = new ExecuteStoredProcedureWithReturn(ProcedureName, new[] { new ProcedureParam(InterfaceParameterName, FolderId)});
            
            DynamicORM orm = new DynamicORM();
            DataSet ds = orm.RunQuery(executeStatement);
            return ds.Tables[0];
        }
    }
}