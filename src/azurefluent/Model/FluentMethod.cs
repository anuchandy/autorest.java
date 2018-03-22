using AutoRest.Core.Model;
using AutoRest.Java.Azure.Fluent.Model;
using AutoRest.Java.Azure.Model;
using AutoRest.Java.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoRest.Java.Azure.Fluent.Model
{
    public class FluentMethod
    {
        private readonly bool isStandard;
        private CompositeTypeJvaf innerReturnType;

        private FluentModel returnModel;

        private FluentModel parameterModel;

        public MethodJvaf InnerMethod { get; private set; }

        public FluentMethodGroup MethodGroup { get; private set; }

        public string Name
        {
            get
            {
                return this.InnerMethod.Name.Value;
            }
        }

        public CompositeTypeJvaf InnerReturnType
        {
            get
            {
                if (innerReturnType == null)
                {
                    IModelType mtype = InnerMethod.ReturnTypeJva.BodyClientType;
                    if (mtype is CompositeTypeJvaf)
                    {
                        this.innerReturnType = (CompositeTypeJvaf)mtype;
                    }
                    else if (mtype is SequenceTypeJva)
                    {
                        mtype = ((SequenceTypeJva)mtype).ElementType;

                        if (mtype is CompositeTypeJvaf)
                        {
                            this.innerReturnType = (CompositeTypeJvaf)mtype;
                        }
                        else
                        {
                            throw new InvalidOperationException($"Expected CompositeTypeJvaf but found {mtype.ClassName}");
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException($"Expected CompositeTypeJvaf but found {mtype.ClassName}");
                    }
                }
                return this.innerReturnType;
            }
        }

        public FluentModel ReturnModel
        {
            get
            {
                if (returnModel == null)
                {
                    if (this.isStandard)
                    {
                        this.returnModel = this.MethodGroup.StandardFluentModel;
                    }
                    else
                    {
                        this.returnModel = new FluentModel(this.InnerReturnType);
                    }
                }
                return this.returnModel;
            }
        }

        public FluentModel ParameterModel
        {
            get
            {
                if (this.parameterModel == null)
                {

                }
                return this.parameterModel;
            }
        }

        public FluentMethod(bool isStandard, MethodJvaf innerMethod, FluentMethodGroup methodGroup)
        {
            this.isStandard = isStandard;
            this.InnerMethod = innerMethod;
            this.MethodGroup = methodGroup;
        }
    }
}
