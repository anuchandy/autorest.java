namespace AutoRest.Java.Azure.Fluent.Model
{
    public class FluentModelMemberVariablesForGet : FluentModelMemberVariables
    {
        public FluentModelMemberVariablesForGet() : base(null)
        {
            this.FluentMethodGroup = null;
        }

        public FluentModelMemberVariablesForGet(FluentMethodGroup fluentMethodGroup) :
            base(fluentMethodGroup.ResourceGetDescription.SupportsGetByImmediateParent ? fluentMethodGroup.ResourceGetDescription.GetByImmediateParentMethod : null)
        {
            this.FluentMethodGroup = fluentMethodGroup;
        }

        public FluentMethodGroup FluentMethodGroup { get; private set; }
    }
}
