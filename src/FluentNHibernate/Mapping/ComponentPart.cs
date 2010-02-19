﻿using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using FluentNHibernate.Mapping.Providers;
using FluentNHibernate.MappingModel;
using FluentNHibernate.MappingModel.ClassBased;

namespace FluentNHibernate.Mapping
{
    public class ComponentPart<T> : ComponentPartBase<T>, IComponentMappingProvider
    {
        private readonly Type entity;
        private readonly AccessStrategyBuilder access;
        private readonly AttributeStore attributes;

        public ComponentPart(Type entity, Member property)
            : this(entity, property.Name, new AttributeStore())
        {}

        private ComponentPart(Type entity, string propertyName, AttributeStore underlyingStore)
            : base(underlyingStore, propertyName)
        {
            attributes = underlyingStore.Clone();
            access = new AccessStrategyBuilder(value => attributes.Set(Attr.Access, value));
            this.entity = entity;

            Insert();
            Update();
        }

        protected override IComponentMapping CreateComponentMappingRoot(AttributeStore store)
        {
            return new ComponentMapping(store)
            {
                ContainingEntityType = entity,
                Class = new TypeReference(typeof(T))
            };
        }

        /// <summary>
        /// Set the access and naming strategy for this component.
        /// </summary>
        public new AccessStrategyBuilder<ComponentPart<T>> Access
        {
            get { return new AccessStrategyBuilder<ComponentPart<T>>(this, access); }
        }

        public new ComponentPart<T> ParentReference(Expression<Func<T, object>> exp)
        {
            base.ParentReference(exp);
            return this;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public new ComponentPart<T> Not
        {
            get
            {
                var forceExecution = base.Not;
                return this;
            }
        }

        public new ComponentPart<T> ReadOnly()
        {
            base.ReadOnly();
            return this;
        }

        public new ComponentPart<T> Insert()
        {
            base.Insert();
            return this;
        }

        public new ComponentPart<T> Update()
        {
            base.Update();
            return this;
        }

        public ComponentPart<T> LazyLoad()
        {
            attributes.Set(Attr.Lazy, nextBool);
            nextBool = true;
            return this;
        }

        public new ComponentPart<T> OptimisticLock()
        {
            base.OptimisticLock();
            return this;
        }

        IComponentMapping IComponentMappingProvider.GetComponentMapping()
        {
            return CreateComponentMapping();
        }
    }
}
