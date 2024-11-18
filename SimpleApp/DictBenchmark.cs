using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SimpleApp;
[MemoryDiagnoser]
[HideColumns("Error", "StdDev", "Median", "RatioSD")]
public class DictBenchmark
{
    Dictionary<string, DictionayDataEle> dicomDictionary;
    FrozenDictionary<string, DictionayDataEle> frozenDicomDictionary;
    List<string> tagsForLookup = new List<string>()
    {
        "00005120",
        "00005130",
        "00005140",
        "00005150",
        "00005160",
        "00005170",
        "00005180",
        "00005190",
        "000051A0",
        "000051B0",
        "00020000",
        "00020001",
        "00020002",
        "00080081",
        "00080082",
        "00080090",
        "00080092",
        "00080094",
        "00080096",
        "00080100",
        "00080102",
        "00080103",
        "00080104",
        "00080105",
        "00080106",
        "00080107",
        "0008010B",
        "0008010C",
        "0008010D",
        "00181040",
        "00181041",
        "00181042",
        "00181043",
        "00181044",
        "00181045",
        "00181046",
        "00181047",
        "00181048",
        "00181049",
        "00181050",
        "00181060",
        "00181061",
        "00181062",
        "00181063",
        "00181064",
        "00181065",
        "00181066",
        "00181067",
        "00181068",
        "00100010",
        "00100020",
        "00100021",
        "00100022",
        "00100024",
        "00100030",
        "00100032",
        "00100040",
        "00100050",
        "00100101",
        "00100102",
        "00101000",
        "00101001",
        "00101002",
        "00101005",
        "00101010",
        "00101020",
        "00101021",
        "00101030",
        "00101040",
        "00101050",
        "00101060",
        "00101080",
        "00101081",
        "00101090",
        "52009230",
        "54000100",
        "54000110",
        "54000112",
        "54001004",
        "54001006",
        "5400100A",
        "54001010",
        "56000010",
        "56000020",
        "7FE00010"
    };
    public DictBenchmark()
    {
        Setup();
    }
    public void Setup()
    {
        var jsonText = File.ReadAllText("standardDataElements.json");
        dicomDictionary = JsonSerializer.Deserialize<Dictionary<string, DictionayDataEle>>(jsonText);
        frozenDicomDictionary = dicomDictionary.ToFrozenDictionary();
    }
    [Benchmark]
    public void RegularDictionaryLookup ()
    {
        foreach (var tag in tagsForLookup)
        {
            if (!dicomDictionary.TryGetValue(tag, out var value))
            {
                throw new Exception("tag not found");
            }
        }
        //if (!dicomDictionary.TryGetValue("00100010", out var value))
        //{
        //    throw new Exception("tag not found");
        //}

        //if (!dicomDictionary.TryGetValue("7FE00010", out var value1))
        //{
        //    throw new Exception("tag not found");
        //}

    }
    [Benchmark(Baseline = true)]
    public void FrozenDictionaryLookup()
    {
        foreach (var tag in tagsForLookup)
        {
            if (!frozenDicomDictionary.TryGetValue(tag, out var value))
            {
                throw new Exception("tag not found");
            }
        }

        //if (!frozenDicomDictionary.TryGetValue("00100010", out var value))
        //{
        //    throw new Exception("tag not found");
        //}

        //if (!frozenDicomDictionary.TryGetValue("7FE00010", out var value1))
        //{
        //    throw new Exception("tag not found");
        //}

    }

}
