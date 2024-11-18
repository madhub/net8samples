// See https://aka.ms/new-console-template for more information

//  "00000000": { tag: "(0000,0000)", vr: "UL", vm: "1", name: "CommandGroupLength" },

//using System.Collections.Frozen;
//using System.Text.Json;
//using System.Text.Json.Serialization;

//var jsonText = File.ReadAllText("standardDataElements.json");
//var objects = JsonSerializer.Deserialize<Dictionary<string, DictionayDataEle>>(jsonText);
//Console.WriteLine(objects.Count);

//var frozenDict =  objects.ToFrozenDictionary();
using BenchmarkDotNet.Running;
using SimpleApp;

var summary = BenchmarkRunner.Run<  >();
