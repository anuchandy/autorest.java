namespace AutoRest.Java.Azure.Fluent.Model
{
    public class NestedFluentModelGetMemeberVariables : FluentModelMemberVariables
    {
        public NestedFluentModelGetMemeberVariables() : base(null)
        {
            this.FluentMethodGroup = null;
        }

        public NestedFluentModelGetMemeberVariables(FluentMethodGroup fluentMethodGroup) :
            base(fluentMethodGroup.ResourceGetDescription.SupportsGetByImmediateParent ? fluentMethodGroup.ResourceGetDescription.GetByImmediateParentMethod : null)
        {
            this.FluentMethodGroup = fluentMethodGroup;
        }

        public FluentMethodGroup FluentMethodGroup { get; private set; }
    }
}
