using System.Data;
using DecisionsFramework.Data.ORMapper;
using DecisionsFramework.Design.Report;
using DecisionsFramework.ServiceLayer.Services.ContextData;

namespace Decisions.TruBot.Data
{
    [AutoRegisterReportElement("TruBot Recorded Bots", "TruBot")]
    public class RecordedBotsDataSource : AbstractCustomDataSource, IFolderAwareFilter
    {
        public string FolderId { get; set; }

        public string TableName { get; }

        public RecordedBotsDataSource()
        {
            TableName = "trubot_recorded_bots";
        }
        
        private readonly string ProcedureName = "GetTruBotEntity";
        private readonly string InterfaceParameterName = "truBotEntityId";

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
                    new ReportFieldData(TableName, "bot_id", "BotId", typeof(string)),
                    new ReportFieldData(TableName, "bot_name", "BotName", typeof(string)),
                    new ReportFieldData(TableName, "initialized_on", "InitializedOn", typeof(DateTime)),
                    new ReportFieldData(TableName, "last_run_on", "LastRunOn", typeof(DateTime))
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