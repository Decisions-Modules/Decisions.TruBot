using DecisionsFramework.Design.Report;
using DecisionsFramework.Design.Report.Service.ORMFilters;

namespace Decisions.TruBot.Data
{
    [AutoRegisterReportElement("TruBot Process Data", "TruBot")]
    public class TruBotProcessDataSource : BaseORMReportSource<TruBotProcess>
    {
        public TruBotProcessDataSource()
        {
            this.ExposeActions = true;
        }
    }
}