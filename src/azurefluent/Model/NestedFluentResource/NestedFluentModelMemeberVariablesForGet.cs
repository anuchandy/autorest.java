namespace AutoRest.Java.Azure.Fluent.Model
{
    public class NestedFluentModelMemeberVariablesForGet : FluentModelMemberVariables
    {
        public NestedFluentModelMemeberVariablesForGet() : base(null)
        {
            this.FluentMethodGroup = null;
        }

        public NestedFluentModelMemeberVariablesForGet(FluentMethodGroup fluentMethodGroup) :
            base(fluentMethodGroup.ResourceGetDescription.SupportsGetByImmediateParent ? fluentMethodGroup.ResourceGetDescription.GetByImmediateParentMethod : null)
        {
            this.FluentMethodGroup = fluentMethodGroup;
        }

        public FluentMethodGroup FluentMethodGroup { get; private set; }
    }
}
