using DecisionsFramework.ServiceLayer.Services.Folder;

namespace Decisions.TruBot.Data
{
    public class TruBotInterfacesFolderBehavior : DefaultFolderBehavior, ISystemFolderBehavior
    {
        internal const string NAME = "TruBot Interfaces";
        
        internal static string GetTruBotFolderId(string projectId) => $"trubot.{projectId}";
    }
}