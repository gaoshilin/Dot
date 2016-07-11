using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dot.Dependency.Engine;
using Autofac;
using Dot.Serializing;
using Dot.Test.Support;
using Dot.Extension;

namespace Dot.Test.Serializing
{
    [TestClass]
    public class JsonNetSerializerTest
    {
        [TestMethod]
        public void Serializing_JsonNetSerializer_Test()
        {
            var serializer = EngineContext.Current.ResolveNamed<JsonNetSerializer>("jsonnet");
            Assert.IsNotNull(serializer);

            var serializer2 = EngineContext.Current.Resolve<IJsonSerializer>();
            Assert.IsNotNull(serializer2);
            Assert.IsInstanceOfType(serializer2, typeof(JsonNetSerializer));
            Assert.IsTrue(object.ReferenceEquals(serializer, serializer2));

            var person = new Person { Name = "tyson", Age = 44, Birthday = "1984-11-02".ToDate() };
            var json = serializer.Serialize(person);
            var person2 = serializer.Deserialize<Person>(json);
            Assert.AreEqual(person.Name, person2.Name);
            Assert.AreEqual(person.Age, person2.Age);
            Assert.AreEqual(person.Birthday, person2.Birthday);
        }
    }

    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public DateTime Birthday { get; set; }
    }
}