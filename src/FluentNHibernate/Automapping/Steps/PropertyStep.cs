using System;
using System.Linq;
using FluentNHibernate.Automapping.Rules;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Mapping;
using FluentNHibernate.MappingModel;
using FluentNHibernate.MappingModel.ClassBased;
using FluentNHibernate.Utils;

namespace FluentNHibernate.Automapping.Steps
{
    public class PropertyStep : IAutomappingStep
    {
        private readonly IConventionFinder conventionFinder;
        private readonly IAutomappingDiscoveryRules rules;

        public PropertyStep(IAutomappingDiscoveryRules rules, IConventionFinder conventionFinder)
        {
            this.conventionFinder = conventionFinder;
            this.rules = rules;
        }

        public bool IsMappable(Member property)
        {
            if (HasExplicitTypeConvention(property))
                return true;

            if (property.CanWrite)
                return IsMappableToColumnType(property);

            return false;
        }

        private bool HasExplicitTypeConvention(Member property)
        {
            // todo: clean this up!
            //        What it's doing is finding if there are any IUserType conventions
            //        that would be applied to this property, if there are then we should
            //        definitely automap it. The nasty part is that right now we don't have
            //        a model, so we're having to create a fake one so the convention will
            //        apply to it.
            var conventions = conventionFinder
                .Find<IPropertyConvention>()
                .Where(c =>
                {
                    if (!typeof(IUserTypeConvention).IsAssignableFrom(c.GetType()))
                        return false;

                    var criteria = new ConcreteAcceptanceCriteria<IPropertyInspector>();
                    var acceptance = c as IConventionAcceptance<IPropertyInspector>;
                    
                    if (acceptance != null)
                        acceptance.Accept(criteria);

                    return criteria.Matches(new PropertyInspector(new PropertyMapping
                    {
                        Type = new TypeReference(property.PropertyType),
                        Member = property
                    }));
                });

            return conventions.FirstOrDefault() != null;
        }

        private static bool IsMappableToColumnType(Member property)
        {
            return property.PropertyType.Namespace == "System"
                || property.PropertyType.FullName == "System.Drawing.Bitmap"
                    || property.PropertyType.IsEnum;
        }

        public void Map(ClassMappingBase classMap, Member member)
        {
            classMap.AddProperty(GetPropertyMapping(classMap.Type, member, classMap as ComponentMapping));
        }

        private PropertyMapping GetPropertyMapping(Type type, Member property, ComponentMapping component)
        {
            var mapping = new PropertyMapping
            {
                ContainingEntityType = type,
                Member = property
            };

            var columnName = property.Name;
            
            if (component != null)
                columnName = rules.ComponentColumnPrefixRule(component.Member) + columnName;

            mapping.AddDefaultColumn(new ColumnMapping { Name = columnName });

            if (!mapping.IsSpecified("Name"))
                mapping.Name = mapping.Member.Name;

            if (!mapping.IsSpecified("Type"))
                mapping.SetDefaultValue("Type", GetDefaultType(property));

            return mapping;
        }

        private TypeReference GetDefaultType(Member property)
        {
            var type = new TypeReference(property.PropertyType);

            if (property.PropertyType.IsEnum())
                type = new TypeReference(typeof(GenericEnumMapper<>).MakeGenericType(property.PropertyType));

            if (property.PropertyType.IsNullable() && property.PropertyType.IsEnum())
                type = new TypeReference(typeof(GenericEnumMapper<>).MakeGenericType(property.PropertyType.GetGenericArguments()[0]));

            return type;
        }

    }
}