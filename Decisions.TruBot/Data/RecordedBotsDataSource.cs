using DecisionsFramework.Design.Report;
using DecisionsFramework.Design.Report.Service.ORMFilters;

namespace Decisions.TruBot.Data
{
    [AutoRegisterReportElement("TruBot Recorded Bots", "TruBot")]
    public class RecordedBotsDataSource : BaseORMReportSource<TruBotRecordedBot>
    {
        public RecordedBotsDataSource()
        {
            this.ExposeActions = true;
        }
    }
}