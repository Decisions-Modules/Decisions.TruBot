using DecisionsFramework.Data.ORMapper;
using DecisionsFramework.Utilities;

namespace Decisions.TruBot.Data
{
    [ORMEntity("trubot_invocation_entity")]
    public class TruBotInvocationEntity : BaseORMEntity
    {
        [ORMPrimaryKeyField]
        public string Id { get; set; }

        [ORMField]
        public string? Status { get; set; }
        
        [ORMField]
        public string FlowTrackingId { get; set; }

        [ORMField]
        public string StepTrackingId { get; set; }

        public override void BeforeSave()
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = IDUtility.GetNewIdString();
            }
            base.BeforeSave();
        }

        static ORM<TruBotInvocationEntity> orm = new();

        private static TruBotInvocationEntity GetTruBotInvocation(string truBotInvocationId)
        {
            return orm.Fetch(new WhereCondition[]
            {
                new FieldWhereCondition("Id", QueryMatchType.Equals, truBotInvocationId)
            }).FirstOrDefault();
        }
    }
}