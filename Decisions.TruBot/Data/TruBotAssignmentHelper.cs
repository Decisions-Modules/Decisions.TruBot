using DecisionsFramework;
using DecisionsFramework.Data.ORMapper;
using DecisionsFramework.ServiceLayer.Services.Assignments;
using DecisionsFramework.ServiceLayer.Services.Folder;
using DecisionsFramework.ServiceLayer.Utilities;

namespace Decisions.TruBot.Data
{
    public class TruBotAssignmentHelper
    {
        private static Log log = new Log("TruBot Assignments");
        
        public static void CreateAssignment(TruBotProcess truBotProcess, string interfaceId = TruBotConstants.TRUBOT_ASSIGNMENT_FOLDER_ID)
        {
            Folder interfaceFolder = FolderService.Instance.GetByID(UserContextHolder.GetCurrent(), interfaceId);
            if (interfaceFolder == null)
            {
                InitializeAssignmentComponents();
            }

            Assignment assignment = new Assignment
            {
                EntityName = $"TruBot process '{truBotProcess.Id}' started at {truBotProcess.StartTime} with status: {truBotProcess.Status}",
                EntityDescription = $"TruBot process {truBotProcess.Id} started",
                StartDate = DecisionsFramework.Utilities.DateUtilities.Now(),
                EntityFolderID = interfaceId,
                Priority = "Medium",
                Handler = new TruBotAssignmentHandler(truBotProcess)
            };
            AssignmentService.Instance.Save(UserContextHolder.GetCurrent(), assignment);
            AssignmentService.Instance.AddAssignmentForRole(UserContextHolder.GetCurrent(), assignment.AssignmentId, TruBotConstants.TRUBOT_ASSIGNMENT_ROLE_ID);
            
            ORM<TruBotProcess> botProcessOrm = new ORM<TruBotProcess>();
            truBotProcess.AssignmentId = assignment.AssignmentId;
            botProcessOrm.Store(truBotProcess);
        }

        public static void InitializeAssignmentComponents()
        {
            ORM<Folder> orm = new ORM<Folder>();
            Folder f = orm.Fetch(TruBotConstants.TRUBOT_ASSIGNMENT_FOLDER_ID);

            if (f == null)
            {
                string folderId = FolderService.Instance.CreateSubFolder(new SystemUserContext(), TruBotConstants.TRUBOT_ASSIGNMENT_FOLDER_ID, "Assignments",
                    typeof(DefaultFolderBehavior).FullName, TruBotConstants.TRUBOT_ROOT_FOLDER_ID);

                if (FolderService.Instance.GetRootNodeByFolderId(new SystemUserContext(), folderId) == null)
                    FolderService.Instance.AddRootNodeFolder(new SystemUserContext(), folderId, true);
            }
            
            ORM<AssignmentRoleType> assignmentRoleTypeORM = new ORM<AssignmentRoleType>();
            if (assignmentRoleTypeORM.Fetch(TruBotConstants.TRUBOT_ASSIGNMENT_ROLE_ID) == null)
            {
                AssignmentRoleType truBotAssignmentRole = new AssignmentRoleType();
                truBotAssignmentRole.EntityName = "TruBot Process Assignment Role";
                truBotAssignmentRole.AssignmentRoleTypeId = TruBotConstants.TRUBOT_ASSIGNMENT_ROLE_ID;
                assignmentRoleTypeORM.Store(truBotAssignmentRole);
            }
        }
    }
}