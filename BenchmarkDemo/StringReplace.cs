using BenchmarkDotNet.Attributes;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenchmarkDemo;
[MemoryDiagnoser]
public class StringReplace
{
    String base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes("Hello madhu")).TrimEnd('=');
    [Benchmark]
    public void UsingString()
    {
        String safeBase64 = base64String.Replace('+', '-').Replace('/', '_');
    }

    [Benchmark]
    public void UsingStringBuilder()
    {
        StringBuilder sb = new StringBuilder(base64String);

        var safeBase64 = sb.Replace('+', '-').Replace('/', '_');
    }
}
