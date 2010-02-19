using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FluentNHibernate.Mapping.Providers;
using FluentNHibernate.MappingModel;
using FluentNHibernate.MappingModel.ClassBased;
using FluentNHibernate.Utils;

namespace FluentNHibernate.Mapping
{
    public abstract class ComponentPartBase<T> : ClasslikeMapBase<T>
    {
        private readonly string propertyName;
        private readonly AccessStrategyBuilder access;
        protected bool nextBool = true;
        private readonly AttributeStore attributes;

        protected ComponentPartBase(AttributeStore underlyingStore, string propertyName)
        {
            attributes = underlyingStore.Clone();
            access = new AccessStrategyBuilder(value => attributes.Set(Attr.Access, value));
            this.propertyName = propertyName;
        }

        protected abstract IComponentMapping CreateComponentMappingRoot(AttributeStore store);
        protected IComponentMapping CreateComponentMapping()
        {
            var mapping = CreateComponentMappingRoot(attributes.Clone());

            mapping.Name = propertyName;

            foreach (var property in properties)
                mapping.AddProperty(property.GetPropertyMapping());

            foreach (var component in components)
                mapping.AddComponent(component.GetComponentMapping());

            foreach (var oneToOne in oneToOnes)
                mapping.AddOneToOne(oneToOne.GetOneToOneMapping());

            foreach (var collection in collections)
                mapping.AddCollection(collection.GetCollectionMapping());

            foreach (var reference in references)
                mapping.AddReference(reference.GetManyToOneMapping());

            foreach (var any in anys)
                mapping.AddAny(any.GetAnyMapping());

            return mapping;
        }

        /// <summary>
        /// Set the access and naming strategy for this component.
        /// </summary>
        public AccessStrategyBuilder<ComponentPartBase<T>> Access
        {
            get { return new AccessStrategyBuilder<ComponentPartBase<T>>(this, access); }
        }

        public ComponentPartBase<T> ParentReference(Expression<Func<T, object>> expression)
        {
            return ParentReference(expression.ToMember());
        }

        private ComponentPartBase<T> ParentReference(Member property)
        {
            attributes.Set(Attr.Parent, new ParentMapping
            {
                Name = property.Name,
                ContainingEntityType = typeof(T)
            });

            return this;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ComponentPartBase<T> Not
        {
            get
            {
                nextBool = !nextBool;
                return this;
            }
        }

        public ComponentPartBase<T> ReadOnly()
        {
            attributes.Set(Attr.Insert, !nextBool);
            attributes.Set(Attr.Update, !nextBool);
            nextBool = true;

            return this;
        }

        public ComponentPartBase<T> Insert()
        {
            attributes.Set(Attr.Insert, nextBool);
            nextBool = true;
            return this;
        }

        public ComponentPartBase<T> Update()
        {
            attributes.Set(Attr.Update, nextBool);
            nextBool = true;
            return this;
        }

        public ComponentPartBase<T> Unique()
        {
            attributes.Set(Attr.Unique, nextBool);
            nextBool = true;
            return this;
        }

        public ComponentPartBase<T> OptimisticLock()
        {
            attributes.Set(Attr.OptimisticLock, nextBool);
            nextBool = true;
            return this;
        }
    }
}