using System;
using System.Collections.Generic;
using FluentNHibernate.Automapping.Rules;
using FluentNHibernate.Automapping.Steps;
using FluentNHibernate.MappingModel;
using FluentNHibernate.MappingModel.ClassBased;
using FluentNHibernate.MappingModel.Collections;
using FluentNHibernate.Utils;

namespace FluentNHibernate.Automapping
{
    public class SimpleTypeCollectionStep : IAutomappingStep
    {
        readonly IAutomappingDiscoveryRules rules;
        readonly AutoKeyMapper keys;
        readonly AutoCollectionCreator collections;

        public SimpleTypeCollectionStep(IAutomappingDiscoveryRules rules)
        {
            this.rules = rules;
            keys = new AutoKeyMapper(rules);
            collections = new AutoCollectionCreator();
        }

        public bool IsMappable(Member property)
        {
            if (!property.PropertyType.IsGenericType)
                return false;

            var childType = property.PropertyType.GetGenericArguments()[0];

            return property.CanWrite &&
                property.PropertyType.ClosesInterface(typeof(IEnumerable<>)) &&
                    (childType.IsPrimitive || childType.In(typeof(string), typeof(DateTime)));
        }

        public void Map(ClassMappingBase classMap, Member property)
        {
            if (property.DeclaringType != classMap.Type)
                return;

            var mapping = collections.CreateCollectionMapping(property.PropertyType);

            mapping.ContainingEntityType = classMap.Type;
            mapping.Member = property;
            mapping.SetDefaultValue(x => x.Name, property.Name);

            keys.SetKey(property, classMap, mapping);
            SetElement(property, classMap, mapping);
        
            classMap.AddCollection(mapping);
        }

        private void SetElement(Member property, ClassMappingBase classMap, ICollectionMapping mapping)
        {
            var element = new ElementMapping
            {
                ContainingEntityType = classMap.Type,
                Type = new TypeReference(property.PropertyType.GetGenericArguments()[0])
            };

            element.AddDefaultColumn(new ColumnMapping { Name = rules.SimpleTypeCollectionValueColumnRule(property) });
            mapping.SetDefaultValue(x => x.Element, element);
        }
    }
}