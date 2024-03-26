using Decisions.TruBot.Behaviors;
using DecisionsFramework.Design.Projects.Dependency;
using DecisionsFramework.ServiceLayer.Services.Projects;
using DecisionsFramework.ServiceLayer.Utilities;

namespace Decisions.TruBot;

public class TruBotDependencyInitializer : IModuleDependencyInitializer
{
    public string ModuleName => "Decisions.TruBot";
    
    public void OnDependencyAdded(string projectId)
    {
        FolderStructureHelper.CreateFolderIfNotExistsAndSendEvent(new SystemUserContext(),
            FolderStructureHelper.GetIntegrationsFolderId(projectId),
            TruBotInterfacesFolderBehavior.GetTruBotFolderId(projectId),
            TruBotInterfacesFolderBehavior.NAME,
            typeof(TruBotInterfacesFolderBehavior).FullName);
    }

    public void OnDependencyRemoved(string projectId)
    {
        FolderStructureHelper.DeleteFolderIfExistsAndSendEvent(new SystemUserContext(),
            TruBotInterfacesFolderBehavior.GetTruBotFolderId(projectId),
            FolderStructureHelper.GetIntegrationsFolderId(projectId));
    }
}