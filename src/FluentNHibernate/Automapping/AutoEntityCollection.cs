using FluentNHibernate.Automapping.Rules;
using FluentNHibernate.Automapping.Steps;
using FluentNHibernate.MappingModel;
using FluentNHibernate.MappingModel.ClassBased;
using FluentNHibernate.MappingModel.Collections;
using FluentNHibernate.Utils;

namespace FluentNHibernate.Automapping
{
    public class AutoEntityCollection : IAutomappingStep
    {
        readonly IAutomappingDiscoveryRules rules;
        readonly AutoKeyMapper keys;
        AutoCollectionCreator collections;

        public AutoEntityCollection(IAutomappingDiscoveryRules rules)
        {
            this.rules = rules;
            keys = new AutoKeyMapper(rules);
            collections = new AutoCollectionCreator();
        }

        public bool IsMappable(Member property)
        {
            return property.CanWrite &&
                property.PropertyType.Namespace.In("System.Collections.Generic", "Iesi.Collections.Generic");
        }

        public void Map(ClassMappingBase classMap, Member property)
        {
            if (property.DeclaringType != classMap.Type)
                return;

            var mapping = collections.CreateCollectionMapping(property.PropertyType);

            mapping.ContainingEntityType = classMap.Type;
            mapping.Member = property;
            mapping.SetDefaultValue(x => x.Name, property.Name);

            SetRelationship(property, classMap, mapping);
            keys.SetKey(property, classMap, mapping);

            classMap.AddCollection(mapping);  
        }

        private void SetRelationship(Member property, ClassMappingBase classMap, ICollectionMapping mapping)
        {
            var relationship = new OneToManyMapping
            {
                Class = new TypeReference(property.PropertyType.GetGenericArguments()[0]),
                ContainingEntityType = classMap.Type
            };

            mapping.SetDefaultValue(x => x.Relationship, relationship);
        }
    }
}