using Decisions.TruBot.Data;
using DecisionsFramework.Design.Report;
using DecisionsFramework.Design.Report.Service.ORMFilters;

namespace Decisions.TruBot.DataSources
{
    [AutoRegisterReportElement("TruBot Recorded Bots", "TruBot")]
    public class RecordedBotsDataSource : BaseORMReportSource<TruBotRecordedBot>, IFolderAwareFilter
    {
        public string FolderId { get; set; }
        
        public RecordedBotsDataSource()
        {
            this.ExposeActions = true;
        }
        
    }
}