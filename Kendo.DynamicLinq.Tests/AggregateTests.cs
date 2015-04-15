using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kendo.DynamicLinq;

namespace Kendo.DynamicLinq.Tests
{
    [TestFixture]
    public class AggregateTests
    {


        [Test]
        public void Test1()
        {
            var collection = new List<Tuple<int>>() {
                new Tuple<int>(-5), 
                new Tuple<int>(3), 
                new Tuple<int>(56), 
                new Tuple<int>(9), 
                new Tuple<int>(10), 
                new Tuple<int>(2)
            }.AsQueryable();

            var result = collection.ToDataSourceResult(50
                , 0
                , new List<Sort>() { new Sort { Field = "Item1", Dir = "asc" } }
                , null
                , new List<Aggregator>() {
                    new Aggregator { Field = "Item1", Aggregate = "sum" },
                    new Aggregator { Field = "Item1", Aggregate = "count" },
                    new Aggregator { Field = "Item1", Aggregate = "average" },
                    new Aggregator { Field = "Item1", Aggregate = "min" },
                    new Aggregator { Field = "Item1", Aggregate = "max" }
                });

            var aggregates = result.Aggregates as dynamic;

            Assert.AreEqual(aggregates.Item1.sum, collection.Sum(p => p.Item1));
            Assert.AreEqual(aggregates.Item1.count, collection.Count());
            Assert.AreEqual(aggregates.Item1.average, collection.Average(p => p.Item1));
            Assert.AreEqual(aggregates.Item1.min, collection.Min(p => p.Item1));
            Assert.AreEqual(aggregates.Item1.max, collection.Max(p => p.Item1));

        }
    }
}
