using Decisions.TruBot.Data;
using DecisionsFramework.Design.Report;
using DecisionsFramework.Design.Report.Service.ORMFilters;

namespace Decisions.TruBot.DataSources
{
    [AutoRegisterReportElement("TruBot Process Data", "TruBot")]
    public class TruBotProcessDataSource : BaseORMReportSource<TruBotProcess>, IFolderAwareFilter
    {
        public string FolderId { get; set; }
        
        public TruBotProcessDataSource()
        {
            this.ExposeActions = true;
        }
    }
}