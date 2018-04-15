namespace AutoRest.Java.Azure.Fluent.Model
{
    public class NestedFluentModelMemberVariablesForGet : FluentModelMemberVariablesForGet
    {
        public NestedFluentModelMemberVariablesForGet() : base()
        {
        }

        public NestedFluentModelMemberVariablesForGet(FluentMethodGroup fluentMethodGroup) : base(fluentMethodGroup, 
            fluentMethodGroup.ResourceGetDescription.SupportsGetByImmediateParent ? 
                fluentMethodGroup.ResourceGetDescription.GetByImmediateParentMethod : null)
        {
        }
    }
}
