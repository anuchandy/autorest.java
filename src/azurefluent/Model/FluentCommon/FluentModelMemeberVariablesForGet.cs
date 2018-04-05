namespace AutoRest.Java.Azure.Fluent.Model
{
    public class FluentModelMemeberVariablesForGet : FluentModelMemberVariables
    {
        public FluentModelMemeberVariablesForGet() : base(null)
        {
            this.FluentMethodGroup = null;
        }

        public FluentModelMemeberVariablesForGet(FluentMethodGroup fluentMethodGroup) :
            base(fluentMethodGroup.ResourceGetDescription.SupportsGetByImmediateParent ? fluentMethodGroup.ResourceGetDescription.GetByImmediateParentMethod : null)
        {
            this.FluentMethodGroup = fluentMethodGroup;
        }

        public FluentMethodGroup FluentMethodGroup { get; private set; }
    }
}
