using System.Data;
using DecisionsFramework.Data.ORMapper;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Report;
using DecisionsFramework.ServiceLayer;
using DecisionsFramework.ServiceLayer.Services.ContextData;

namespace Decisions.TruBot.Data
{
    [AutoRegisterReportElement("TruBot Process Data", "TruBot")]
    [Writable]
    public class TruBotProcessDataSource : AbstractCustomDataSource
    {
        public string TableName { get; }

        public TruBotProcessDataSource()
        {
            TableName = "trubot_process_data";
        }

        public override bool Applies(ReportDefinition definition)
        {
            return definition.HasDataSourcesOrFilters() == false;
        }

        public override ReportFieldData[] ReportFields
        {
            get
            {
                return new[]
                {
                    new ReportFieldData(TableName, "Workflow_Name", typeof(string)),
                    new ReportFieldData(TableName, "Bot_Name", typeof(string)),
                    new ReportFieldData(TableName, "Start_Time", typeof(DateTime)),
                    new ReportFieldData(TableName, "Step_Duration", typeof(TimeOnly))
                };
            }
        }

        public DataTable GetData(DataTable table, IReportFilter[] filters, int? limitCount, int pageIndex, DataPair[] sortingInfo, Func<DataRow, bool> rowHandler)
        {
            if (table == null)
            {
                table = new DataTable();
            }

            table.Columns.AddRange(GetColumnsFromReportFields(ReportFields));

            CompositeSelectStatement statement = new CompositeSelectStatement(
                new CompositeSelectStatement.TableDefinition("trubot_process_data"));
            
            statement.PrimaryTable.Fields.Add(new CompositeSelectStatement.FieldDefinition("Workflow_Name"));
            statement.PrimaryTable.Fields.Add(new CompositeSelectStatement.FieldDefinition("Bot_Name"));
            statement.PrimaryTable.Fields.Add(new CompositeSelectStatement.FieldDefinition("Start_Time"));
            statement.PrimaryTable.Fields.Add(new CompositeSelectStatement.FieldDefinition("Step_Duration"));

            foreach (var sortInfo in sortingInfo)
            {
                if (Enum.TryParse(sortInfo.OutputValue?.ToString(), out ORMResultOrder direction))
                {
                    statement.OrderBy.Add(sortInfo.Name, direction);
                }
            }

            int? startIndex = null;

            if (limitCount.HasValue)
            {
                startIndex = pageIndex * limitCount;
            }

            if (rowHandler == null && limitCount != null)
            {
                statement.Top = (pageIndex + 1) * limitCount;
            }


            IDbConnection? conn = null;
            if (DynamicORM.DatabaseDriver.DatabaseType == DataBaseTypeEnum.POSTGRES)
            {
                //conn = new NpgsqlConnection(DynamicORM.ConnectionString);
            }
            else
            {
                //conn = ConnectionUtilities.GetSQLConnection(DynamicORM.ConnectionString);
            }
            
            using (conn)
            {
                conn.Open();
                IDataReader reader = statement.GetQueryResult(conn);
                int index = 0;

                while (reader.Read())
                {
                    if (table.Rows.Count >= limitCount) break;

                    DataRow dr = table.NewRow();
                    for (int i = 0; i < ReportFields.Length; i++)
                    {
                        dr[ReportFields[i].FieldName] = reader[i];
                    }

                    if (rowHandler == null || rowHandler(dr))
                    {
                        bool shouldAdd = true;

                        if (startIndex.HasValue)
                        {
                            shouldAdd = index >= startIndex;
                        }

                        if (shouldAdd)
                        {
                            table.Rows.Add(dr);
                        }

                        index++;
                    }
                }
            }

            return table;
        }
    }
}