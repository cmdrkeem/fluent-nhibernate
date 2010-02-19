using System;
using System.Diagnostics;
using System.Reflection;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Mapping;
using FluentNHibernate.MappingModel;
using FluentNHibernate.MappingModel.Collections;
using NHibernate.Persister.Entity;

namespace FluentNHibernate.Conventions.Instances
{
    public class CollectionInstance : CollectionInspector, ICollectionInstance
    {
        private readonly ICollectionMapping mapping;
        protected bool nextBool = true;

        public CollectionInstance(ICollectionMapping mapping)
            : base(mapping)
        {
            this.mapping = mapping;
        }

        public new IRelationshipInstance Relationship
        {
            get
            {
                if (mapping.Relationship is ManyToManyMapping)
                    return new ManyToManyInstance((ManyToManyMapping)mapping.Relationship);

                return new OneToManyInstance((OneToManyMapping)mapping.Relationship);
            }
        }

        public new ICollectionCascadeInstance Cascade
        {
            get
            {
                return new CollectionCascadeInstance(value =>
                {
                    if (!mapping.IsSpecified(Attr.Cascade))
                        mapping.Cascade = value;
                });
            }
        }

        public new IFetchInstance Fetch
        {
            get
            {
                return new FetchInstance(value =>
                {
                    if (!mapping.IsSpecified(Attr.Fetch))
                        mapping.Fetch = value;
                });
            }
        }

        public new IOptimisticLockInstance OptimisticLock
        {
            get
            {
                return new OptimisticLockInstance(value =>
                {
                    if (!mapping.IsSpecified(Attr.OptimisticLock))
                        mapping.OptimisticLock = value;
                });
            }
        }

        public new void Check(string constraint)
        {
            if (!mapping.IsSpecified(Attr.Check))
                mapping.Check = constraint;
        }

        public new void CollectionType<T>()
        {
            if (!mapping.IsSpecified(Attr.CollectionType))
                mapping.CollectionType = new TypeReference(typeof(T));
        }

        public new void CollectionType(string type)
        {
            if (!mapping.IsSpecified(Attr.CollectionType))
                mapping.CollectionType = new TypeReference(type);
        }

        public new void CollectionType(Type type)
        {
            if (!mapping.IsSpecified(Attr.CollectionType))
                mapping.CollectionType = new TypeReference(type);
        }

        public new void Generic()
        {
            if (!mapping.IsSpecified(Attr.Generic))
                mapping.Generic = nextBool;
            nextBool = true;
        }

        public new void Inverse()
        {
            if (!mapping.IsSpecified(Attr.Inverse))
                mapping.Inverse = nextBool;
            nextBool = true;
        }

        public new void Persister<T>() where T : IEntityPersister
        {
            if (!mapping.IsSpecified(Attr.Persister))
                mapping.Persister = new TypeReference(typeof(T));
        }

        public new void Where(string whereClause)
        {
            if (!mapping.IsSpecified(Attr.Where))
                mapping.Where = whereClause;
        }

        public new void OrderBy(string orderBy)
        {
            if (!mapping.IsSpecified(Attr.OrderBy))
                mapping.OrderBy = orderBy;
        }

        public void Subselect(string subselect)
        {
            if (!mapping.IsSpecified(Attr.Subselect))
                mapping.Subselect = subselect;
        }

        public void Table(string tableName)
        {
            if (!mapping.IsSpecified(Attr.Table))
                mapping.TableName = tableName;
        }

        public new void Name(string name)
        {
            if (!mapping.IsSpecified(Attr.Name))
                mapping.Name = name;
        }

        public new void Schema(string schema)
        {
            if (!mapping.IsSpecified(Attr.Schema))
                mapping.Schema = schema;
        }

        public new void LazyLoad()
        {
            if (!mapping.IsSpecified(Attr.Lazy))
                mapping.Lazy = nextBool;
            nextBool = true;
        }

        public new void BatchSize(int batchSize)
        {
            if (!mapping.IsSpecified(Attr.BatchSize))
                mapping.BatchSize = batchSize;
        }

        public void ReadOnly()
        {
            if (!mapping.IsSpecified(Attr.Mutable))
                mapping.Mutable = !nextBool;
            nextBool = true;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ICollectionInstance Not
        {
            get 
            {
                nextBool = !nextBool;
                return this; 
            }
        }
        
        public new ICacheInstance Cache
        {
            get
            {
                if (mapping.Cache == null)
                    // conventions are hitting it, user must want a cache
                    mapping.Cache = new CacheMapping();

                return new CacheInstance(mapping.Cache);
            }
        }

        public new IAccessInstance Access
        {
            get
            {
                return new AccessInstance(value =>
                {
                    if (!mapping.IsSpecified(Attr.Access))
                        mapping.Access = value;
                });
            }
        }

        public new IKeyInstance Key
        {
            get { return new KeyInstance(mapping.Key); }
        }
    }
}