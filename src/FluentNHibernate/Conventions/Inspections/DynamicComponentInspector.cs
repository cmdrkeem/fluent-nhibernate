using System.Reflection;
using FluentNHibernate.Mapping;
using FluentNHibernate.MappingModel;
using FluentNHibernate.MappingModel.ClassBased;

namespace FluentNHibernate.Conventions.Inspections
{
    public class DynamicComponentInspector : ComponentBaseInspector, IDynamicComponentInspector
    {
        private readonly InspectorModelMapper<IDynamicComponentInspector, DynamicComponentMapping> mappedProperties = new InspectorModelMapper<IDynamicComponentInspector, DynamicComponentMapping>();
        private readonly DynamicComponentMapping mapping;

        public DynamicComponentInspector(DynamicComponentMapping mapping)
            : base(mapping)
        {
            this.mapping = mapping;
        }

        public override bool IsSet(Attr property)
        {
            return mapping.IsSpecified(property);
        }
    }
}