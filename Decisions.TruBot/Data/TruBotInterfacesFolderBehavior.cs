using DecisionsFramework.ServiceLayer.Services.Folder;
using DecisionsFramework.ServiceLayer.Utilities;

namespace Decisions.TruBot.Data
{
    public class TruBotInterfacesFolderBehavior : DefaultFolderBehavior, ISystemFolderBehavior
    {
        internal const string NAME = "TruBot Interfaces";
        
        internal static string GetTruBotFolderId(string projectId) => $"trubot.{projectId}";

        public override ViewPageData[] GetViewPages(Folder folder = null)
        {
            return base.GetViewPages(FolderService.Instance.GetByID(UserContextHolder.GetCurrent(), "01HPJ54BNTZKMBEHX6A6MCZ7E7"));
        }
    }
}