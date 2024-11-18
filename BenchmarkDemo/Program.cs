// See https://aka.ms/new-console-template for more information
using BenchmarkDemo;
using BenchmarkDotNet.Running;
using System.Net.Sockets;

var summary = BenchmarkRunner.Run<BenchmarkDemo.StringReplace>();
