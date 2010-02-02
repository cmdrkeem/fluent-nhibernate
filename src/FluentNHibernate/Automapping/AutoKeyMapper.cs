using FluentNHibernate.Automapping.Rules;
using FluentNHibernate.MappingModel;
using FluentNHibernate.MappingModel.ClassBased;
using FluentNHibernate.MappingModel.Collections;

namespace FluentNHibernate.Automapping
{
    public class AutoKeyMapper
    {
        readonly IAutomappingDiscoveryRules rules;

        public AutoKeyMapper(IAutomappingDiscoveryRules rules)
        {
            this.rules = rules;
        }

        public void SetKey(Member property, ClassMappingBase classMap, ICollectionMapping mapping)
        {
            var columnName = property.DeclaringType.Name + "_id";

            if (classMap is ComponentMapping)
                columnName = rules.ComponentColumnPrefixRule(((ComponentMapping)classMap).Member) + columnName;

            var key = new KeyMapping();

            key.ContainingEntityType = classMap.Type;
            key.AddDefaultColumn(new ColumnMapping { Name = columnName });

            mapping.SetDefaultValue(x => x.Key, key);
        }
    }
}