namespace AutoRest.Java.Azure.Fluent.Model
{
    public class FluentNestedModelGetMemeberVariables : FluentModelMemberVariables
    {
        public FluentNestedModelGetMemeberVariables() : base(null)
        {
            this.FluentMethodGroup = null;
        }

        public FluentNestedModelGetMemeberVariables(FluentMethodGroup fluentMethodGroup) :
            base(fluentMethodGroup.ResourceGetDescription.SupportsGetByImmediateParent ? fluentMethodGroup.ResourceGetDescription.GetByImmediateParentMethod : null)
        {
            this.FluentMethodGroup = fluentMethodGroup;
        }

        public FluentMethodGroup FluentMethodGroup { get; private set; }
    }
}
