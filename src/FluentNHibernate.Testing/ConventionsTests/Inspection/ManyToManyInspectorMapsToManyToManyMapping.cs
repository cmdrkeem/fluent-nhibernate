using System;
using System.Linq;
using System.Linq.Expressions;
using FluentNHibernate.Automapping.TestFixtures;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.MappingModel;
using FluentNHibernate.MappingModel.Collections;
using FluentNHibernate.Utils.Reflection;
using FluentNHibernate.Testing.Utils;
using NUnit.Framework;

namespace FluentNHibernate.Testing.ConventionsTests.Inspection
{
    [TestFixture, Category("Inspection DSL")]
    public class ManyToManyInspectorMapsToManyToManyMapping
    {
        private ManyToManyMapping mapping;
        private IManyToManyInspector inspector;

        [SetUp]
        public void CreateDsl()
        {
            mapping = new ManyToManyMapping();
            inspector = new ManyToManyInspector(mapping);
        }

        [Test]
        public void ChildTypeMapped()
        {
            mapping.ChildType = typeof(ExampleClass);
            inspector.ChildType.ShouldEqual(typeof(ExampleClass));
        }

        [Test]
        public void ChildTypeIsSet()
        {
            mapping.ChildType = typeof(ExampleClass);
            inspector.IsSet(Attr.ChildType)
                .ShouldBeTrue();
        }

        [Test]
        public void ChildTypeIsNotSet()
        {
            inspector.IsSet(Attr.ChildType)
                .ShouldBeFalse();
        }

        [Test]
        public void ClassMapped()
        {
            mapping.Class = new TypeReference(typeof(ExampleClass));
            inspector.Class.ShouldEqual(new TypeReference(typeof(ExampleClass)));
        }

        [Test]
        public void ClassIsSet()
        {
            mapping.Class = new TypeReference(typeof(ExampleClass));
            inspector.IsSet(Attr.Class)
                .ShouldBeTrue();
        }

        [Test]
        public void ClassIsNotSet()
        {
            inspector.IsSet(Attr.Class)
                .ShouldBeFalse();
        }

        [Test]
        public void ColumnsCollectionHasSameCountAsMapping()
        {
            mapping.AddColumn(new ColumnMapping());
            inspector.Columns.Count().ShouldEqual(1);
        }

        [Test]
        public void ColumnsCollectionOfInspectors()
        {
            mapping.AddColumn(new ColumnMapping());
            inspector.Columns.First().ShouldBeOfType<IColumnInspector>();
        }

        [Test]
        public void ColumnsCollectionIsEmpty()
        {
            inspector.Columns.IsEmpty().ShouldBeTrue();
        }

        [Test]
        public void FetchMapped()
        {
            mapping.Fetch = "join";
            inspector.Fetch.ShouldEqual(Fetch.Join);
        }

        [Test]
        public void FetchIsSet()
        {
            mapping.Fetch = "join";
            inspector.IsSet(Attr.Fetch)
                .ShouldBeTrue();
        }

        [Test]
        public void FetchIsNotSet()
        {
            inspector.IsSet(Attr.Fetch)
                .ShouldBeFalse();
        }

        [Test]
        public void ForeignKeyMapped()
        {
            mapping.ForeignKey = "key";
            inspector.ForeignKey.ShouldEqual("key");
        }

        [Test]
        public void ForeignKeyIsSet()
        {
            mapping.ForeignKey = "key";
            inspector.IsSet(Attr.ForeignKey)
                .ShouldBeTrue();
        }

        [Test]
        public void ForeignKeyIsNotSet()
        {
            inspector.IsSet(Attr.ForeignKey)
                .ShouldBeFalse();
        }

        [Test]
        public void LazyMapped()
        {
            mapping.Lazy = true;
            inspector.LazyLoad.ShouldEqual(true);
        }

        [Test]
        public void LazyIsSet()
        {
            mapping.Lazy = true;
            inspector.IsSet(Attr.Lazy)
                .ShouldBeTrue();
        }

        [Test]
        public void LazyIsNotSet()
        {
            inspector.IsSet(Attr.Lazy)
                .ShouldBeFalse();
        }

        [Test]
        public void NotFoundMapped()
        {
            mapping.NotFound = "exception";
            inspector.NotFound.ShouldEqual(NotFound.Exception);
        }

        [Test]
        public void NotFoundIsSet()
        {
            mapping.NotFound = "exception";
            inspector.IsSet(Attr.NotFound)
                .ShouldBeTrue();
        }

        [Test]
        public void NotFoundIsNotSet()
        {
            inspector.IsSet(Attr.NotFound)
                .ShouldBeFalse();
        }

        [Test]
        public void ParentTypeMapped()
        {
            mapping.ParentType = typeof(ExampleClass);
            inspector.ParentType.ShouldEqual(typeof(ExampleClass));
        }

        [Test]
        public void ParentTypeIsSet()
        {
            mapping.ParentType = typeof(ExampleClass);
            inspector.IsSet(Attr.ParentType)
                .ShouldBeTrue();
        }

        [Test]
        public void ParentTypeIsNotSet()
        {
            inspector.IsSet(Attr.ParentType)
                .ShouldBeFalse();
        }

        [Test]
        public void WhereMapped()
        {
            mapping.Where = "x = 1";
            inspector.Where.ShouldEqual("x = 1");
        }

        [Test]
        public void WhereIsSet()
        {
            mapping.Where = "x = 1";
            inspector.IsSet(Attr.Where)
                .ShouldBeTrue();
        }

        [Test]
        public void WhereIsNotSet()
        {
            inspector.IsSet(Attr.Where)
                .ShouldBeFalse();
        }

        #region Helpers

        private Member Prop(Expression<Func<IManyToManyInspector, object>> propertyExpression)
        {
            return ReflectionHelper.GetMember(propertyExpression);
        }

        #endregion
    }
}