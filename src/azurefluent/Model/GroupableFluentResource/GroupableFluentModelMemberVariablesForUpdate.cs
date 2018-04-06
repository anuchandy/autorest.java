using System.Collections.Generic;

namespace AutoRest.Java.Azure.Fluent.Model
{
    public class GroupableFluentModelMemberVariablesForUpdate : FluentModelMemberVariablesForUpdate
    {
        public GroupableFluentModelMemberVariablesForUpdate() : base()
        {
        }

        public GroupableFluentModelMemberVariablesForUpdate(FluentMethodGroup fluentMethodGroup) : 
            base (fluentMethodGroup, ARMTrackedResourceProperties)
        {
        }

        private static List<string> ARMTrackedResourceProperties
        {
            get
            {
                return new List<string>
                {
                    "id",
                    "type",
                    "name",
                    "location",
                    "tags"
                };
            }
        }
    }
}
