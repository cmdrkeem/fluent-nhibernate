using System;
using System.Collections.Generic;
using FluentNHibernate.Automapping.Rules;
using FluentNHibernate.MappingModel;
using FluentNHibernate.MappingModel.ClassBased;

namespace FluentNHibernate.Automapping.Steps
{
    public class OneToManyStep : IAutomappingStep
    {
        readonly SimpleTypeCollectionStep simpleTypeCollectionStep;
        readonly AutoEntityCollection entityCollectionStep;

        public OneToManyStep(IAutomappingDiscoveryRules rules)
        {
            simpleTypeCollectionStep = new SimpleTypeCollectionStep(rules);
            entityCollectionStep = new AutoEntityCollection(rules);
        }

        public bool IsMappable(Member property)
        {
            return simpleTypeCollectionStep.IsMappable(property) ||
                   entityCollectionStep.IsMappable(property);
        }

        public void Map(ClassMappingBase classMap, Member member)
        {
            if (member.DeclaringType != classMap.Type)
                return;

            if (simpleTypeCollectionStep.IsMappable(member))
                simpleTypeCollectionStep.Map(classMap, member);
            else if (entityCollectionStep.IsMappable(member))
                entityCollectionStep.Map(classMap, member);
        }
    }
}