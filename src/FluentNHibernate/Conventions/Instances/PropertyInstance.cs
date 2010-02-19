using System;
using System.Diagnostics;
using System.Linq;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.MappingModel;
using NHibernate.Type;
using NHibernate.UserTypes;

namespace FluentNHibernate.Conventions.Instances
{
    public class PropertyInstance : PropertyInspector, IPropertyInstance
    {
        private readonly PropertyMapping mapping;
        private bool nextBool = true;

        public PropertyInstance(PropertyMapping mapping)
            : base(mapping)
        {
            this.mapping = mapping;
        }

        public new void Insert()
        {
            if (!mapping.IsSpecified(Attr.Insert))
                mapping.Insert = nextBool;
            nextBool = true;
        }

        public new void Update()
        {
            if (!mapping.IsSpecified(Attr.Update))
                mapping.Update = nextBool;
            nextBool = true;
        }

        public new void ReadOnly()
        {
            if (!mapping.IsSpecified(Attr.Insert) && !mapping.IsSpecified(Attr.Update))
                mapping.Insert = mapping.Update = !nextBool;
            nextBool = true;
        }

        public new void Nullable()
        {
            if (!mapping.Columns.First().IsSpecified(Attr.NotNull))
                foreach (var column in mapping.Columns)
                    column.NotNull = !nextBool;

            nextBool = true;
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

        public void CustomType(TypeReference type)
        {
            if (!mapping.IsSpecified(Attr.Type))
            {
                mapping.Type = type;

                if (typeof(ICompositeUserType).IsAssignableFrom(mapping.Type.GetUnderlyingSystemType()))
                    AddColumnsForCompositeUserType();
            }
        }

        public void CustomType<T>()
        {
            CustomType(typeof(T));
        }

        public void CustomType(Type type)
        {
            CustomType(new TypeReference(type));
        }

        public void CustomType(string type)
        {
            CustomType(new TypeReference(type));
        }

        public void CustomSqlType(string sqlType)
        {
            if (mapping.Columns.First().IsSpecified(Attr.SqlType))
                return;
         
            foreach (var column in mapping.Columns)
                column.SqlType = sqlType;
        }

        public new void Precision(int precision)
        {
            if (mapping.Columns.First().IsSpecified(Attr.Precision))
                return;

            foreach (var column in mapping.Columns)
                column.Precision = precision;
        }

        public new void Scale(int scale)
        {
            if (mapping.Columns.First().IsSpecified(Attr.Scale))
                return;

            foreach (var column in mapping.Columns)
                column.Scale = scale;
        }

        public new void Default(string value)
        {
            if (mapping.Columns.First().IsSpecified(Attr.Default))
                return;

            foreach (var column in mapping.Columns)
                column.Default = value;
        }

        public new void Unique()
        {
            if (!mapping.Columns.First().IsSpecified(Attr.Unique))
                foreach (var column in mapping.Columns)
                    column.Unique = nextBool;

            nextBool = true;
        }

        public new void UniqueKey(string keyName)
        {
            if (mapping.Columns.First().IsSpecified(Attr.UniqueKey))
                return;

            foreach (var column in mapping.Columns)
                column.UniqueKey = keyName;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IPropertyInstance Not
        {
            get
            {
                nextBool = !nextBool;
                return this;
            }
        }

        public void Column(string columnName)
        {
            if (mapping.Columns.UserDefined.Count() > 0)
                return;

            var originalColumn = mapping.Columns.FirstOrDefault();
            var column = originalColumn == null ? new ColumnMapping() : originalColumn.Clone();

            column.Name = columnName;

            mapping.ClearColumns();
            mapping.AddColumn(column);
        }

        public new void Formula(string formula)
        {
            if (!mapping.IsSpecified(Attr.Formula))
                mapping.Formula = formula;
        }

        public new IGeneratedInstance Generated
        {
            get
            {
                return new GeneratedInstance(value =>
                {
                    if (!mapping.IsSpecified(Attr.Generated))
                        mapping.Generated = value;
                });
            }
        }

        public new void OptimisticLock()
        {
            if (!mapping.IsSpecified(Attr.OptimisticLock))
                mapping.OptimisticLock = nextBool;
            nextBool = true;
        }

        public new void Length(int length)
        {
            if (mapping.Columns.First().IsSpecified(Attr.Length))
                return;

            foreach (var column in mapping.Columns)
                column.Length = length;
        }

        public new void LazyLoad()
        {
            if (!mapping.IsSpecified(Attr.Lazy))
                mapping.Lazy = nextBool;
            nextBool = true;
        }

        public new void Index(string value)
        {
            if (mapping.Columns.First().IsSpecified(Attr.Index))
                return;

            foreach (var column in mapping.Columns)
                column.Index = value;
        }

        public new void Check(string constraint)
        {
            if (mapping.Columns.First().IsSpecified(Attr.Check))
                return;

            foreach (var column in mapping.Columns)
                column.Check = constraint;
        }

        private void AddColumnsForCompositeUserType()
        {
            var inst = (ICompositeUserType)Activator.CreateInstance(mapping.Type.GetUnderlyingSystemType());

            if (inst.PropertyNames.Length > 1)
            {
                var existingColumn = mapping.Columns.Single();
                mapping.ClearColumns();
                var propertyPrefix = existingColumn.Name;
                for (int i = 0; i < inst.PropertyNames.Length; i++)
                {
                    var propertyName = inst.PropertyNames[i];
                    var propertyType = inst.PropertyTypes[i];

                    var column = existingColumn.Clone();
                    column.Name = propertyPrefix + "_" + propertyName;
                    mapping.AddColumn(column);
                }
            }
        }
    }
}