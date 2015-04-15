using NUnit.Framework;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Kendo.DynamicLinq.Tests
{
    [KnownType(typeof(Person))]
    public class Person
    {
        public int Age { get; set; }
    }

    [TestFixture]
    public class SerializationTests
    {
        [Test]
        public void DataContractJsonSerializerSerializesEmptyAggregates()
        {
            using (var stream = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(typeof(DataSourceResult));

                serializer.WriteObject(stream, new[] { "foo" }.AsQueryable<string>().ToDataSourceResult(1, 0, null, null));

                Assert.AreEqual("{\"Aggregates\":null,\"Data\":[\"foo\"],\"Total\":1}", Encoding.UTF8.GetString(stream.ToArray()));
            }
        }

        [Test]
        public void DataContractJsonSerializerSerializesAggregates()
        {
            using (var stream = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(typeof(DataSourceResult), new DataContractJsonSerializerSettings()
                {
                    UseSimpleDictionaryFormat = true,
                    EmitTypeInformation = EmitTypeInformation.Always,
                    KnownTypes = new [] { typeof (Person), typeof (Dictionary<string, object>) }
                });
                var people = new[] { new Person { Age = 30 }, new Person { Age = 30 } };


                var newjson = Newtonsoft.Json.JsonConvert.SerializeObject(new DataSourceRequest {
                    Filter = new Filter { Field = "LocationId", Operator = "in", Value = new List<int>() { 1, 2, 3} }
                }, Formatting.None, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Full
                });

                serializer.WriteObject(stream, people.AsQueryable().ToDataSourceResult(1, 2, null, null, new [] { new Aggregator { 
                    Aggregate = "sum",
                    Field = "Age"
                } }));

                var json = Encoding.UTF8.GetString(stream.ToArray());

                Assert.AreEqual("{\"Aggregates\":{\"Age\":{\"sum\":60}},\"Data\":[],\"Total\":2}", json);
            }
        }
    }
}
