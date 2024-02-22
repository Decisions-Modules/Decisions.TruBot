using DecisionsFramework.Data.ORMapper;
using DecisionsFramework.ServiceLayer;
using DecisionsFramework.ServiceLayer.Services.Assignments;
using DecisionsFramework.ServiceLayer.Services.Folder;
using DecisionsFramework.ServiceLayer.Utilities;

namespace Decisions.TruBot.Data
{
    public class TruBotInitializer : IInitializable
    {
        public void Initialize()
        {
            TruBotAssignmentHelper.InitializeAssignmentComponents();
        }
    }
}