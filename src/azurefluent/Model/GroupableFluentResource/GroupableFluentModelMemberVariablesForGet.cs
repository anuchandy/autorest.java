using System;
using System.Collections.Generic;
using System.Text;

namespace AutoRest.Java.Azure.Fluent.Model
{
    public class GroupableFluentModelMemberVariablesForGet : FluentModelMemberVariablesForGet
    {
        public GroupableFluentModelMemberVariablesForGet() : base()
        {
        }

        public GroupableFluentModelMemberVariablesForGet(FluentMethodGroup fluentMethodGroup) : base(fluentMethodGroup, 
            fluentMethodGroup.ResourceGetDescription.SupportsGetByResourceGroup ? 
                fluentMethodGroup.ResourceGetDescription.GetByResourceGroupMethod : null)
        {
        }
    }
}
