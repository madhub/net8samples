using BenchmarkDotNet.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BenchmarkDemo;
[MemoryDiagnoser]
public class Benchmarker
{
    Person person = new Person()
    {
        Id = 1,
        Name = "Test",
        Age = 30,
        IsStudent = true,
        Interests = new string[] { "Programming", "Gaming", "Reading" },
        Address = new Address()
        {
            Street = "3rd cross 1 main ",
            City = "Bangalore",
            Zipcode = "560070"
        }

    };
    public Benchmarker()
    {
      
    }

    [Benchmark]
    public void NewtonsoftSerializer()
    {
        JsonConvert.SerializeObject(person);
    }
    [Benchmark]
    public void SystemTextSerializer()
    {
        System.Text.Json.JsonSerializer.Serialize(person);
    }
}
