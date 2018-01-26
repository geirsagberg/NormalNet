using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using FizzWare.NBuilder;

namespace NormalNet.BenchMark
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<SimpleBenchmark>();
        }
    }

    public class SimpleBenchmark
    {
        private readonly IList<OfficeViewModel> data;
        private readonly Normalizer normalizer = new Normalizer();

        public SimpleBenchmark()
        {
            data = Builder<OfficeViewModel>.CreateListOfSize(N).Build();
        }

        //[Params(1000, 10000)]
        public int N { get; set; } = 1000;

        [Benchmark]
        public Dictionary<string, object> Normalize() => normalizer.Normalize(data);
    }

    public class OfficeViewModel
    {
        public Address Address { get; set; }
        public Person Owner { get; set; }
    }

    public class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Address HomeAddress { get; set; }
    }

    public class Address
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
    }
}