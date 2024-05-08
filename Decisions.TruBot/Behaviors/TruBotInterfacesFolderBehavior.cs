using DecisionsFramework.ServiceLayer.Services.Folder;

namespace Decisions.TruBot.Behaviors
{
    public class TruBotInterfacesFolderBehavior : DefaultFolderBehavior, ISystemFolderBehavior
    {
        internal const string NAME = "TruBot Interfaces";
        private const string TRUBOT_INTERFACES_PAGE_ID = "01HPJ3FHTY2HD3SDWX5JNRZ0YN";
        
        internal static string GetTruBotFolderId(string projectId) => $"trubot.{projectId}";

        public override ViewPageData[] GetViewPages(Folder folder = null)
        {
            List<ViewPageData> viewPages = new List<ViewPageData>();
            viewPages.AddRange(base.GetViewPages(folder));
            
            viewPages.Add(new SilverlightPortalPage()
            {
                ConfigurationStorageID = TRUBOT_INTERFACES_PAGE_ID, ViewPageName = "Interfaces",
                DisplayType = new[] { DisplayType.HtmlMobile, DisplayType.Html }
            });

            return viewPages.ToArray();
        }
    }
}