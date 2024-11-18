// See https://aka.ms/new-console-template for more information
using System.Text.Json.Serialization;
using System.Text.Json;

namespace SourceGen
{
    public class WeatherForecast
    {
        public DateTime Date { get; set; }
        public int TemperatureCelsius { get; set; }
        public string? Summary { get; set; }
    }

    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(WeatherForecast))]
    internal partial class SourceGenerationContext : JsonSerializerContext
    {
    }

    public class Program
    {
        public static void Main()
        {
            string jsonString = """
                {
                    "Date": "2019-08-01T00:00:00",
                    "TemperatureCelsius": 25,
                    "Summary": "Hot"
                }
                """;
            WeatherForecast? weatherForecast;

            weatherForecast = JsonSerializer.Deserialize<WeatherForecast>(
                jsonString, SourceGenerationContext.Default.WeatherForecast);
            Console.WriteLine($"Date={weatherForecast?.Date}");
            // output:
            //Date=8/1/2019 12:00:00 AM

            weatherForecast = JsonSerializer.Deserialize(
                jsonString, typeof(WeatherForecast), SourceGenerationContext.Default)
                as WeatherForecast;
            Console.WriteLine($"Date={weatherForecast?.Date}");
            // output:
            //Date=8/1/2019 12:00:00 AM

            var sourceGenOptions = new JsonSerializerOptions
            {
                TypeInfoResolver = SourceGenerationContext.Default
            };
            weatherForecast = JsonSerializer.Deserialize(
                jsonString, typeof(WeatherForecast), sourceGenOptions)
                as WeatherForecast;
            Console.WriteLine($"Date={weatherForecast?.Date}");
            // output:
            //Date=8/1/2019 12:00:00 AM

            jsonString = JsonSerializer.Serialize(
                weatherForecast!, SourceGenerationContext.Default.WeatherForecast);
            Console.WriteLine(jsonString);
            // output:
            //{"Date":"2019-08-01T00:00:00","TemperatureCelsius":25,"Summary":"Hot"}

            jsonString = JsonSerializer.Serialize(
                weatherForecast, typeof(WeatherForecast), SourceGenerationContext.Default);
            Console.WriteLine(jsonString);
            // output:
            //{"Date":"2019-08-01T00:00:00","TemperatureCelsius":25,"Summary":"Hot"}

            sourceGenOptions = new JsonSerializerOptions
            {
                TypeInfoResolver = SourceGenerationContext.Default
            };

            jsonString = JsonSerializer.Serialize(
                weatherForecast, typeof(WeatherForecast), sourceGenOptions);
            Console.WriteLine(jsonString);
            // output:
            //{"Date":"2019-08-01T00:00:00","TemperatureCelsius":25,"Summary":"Hot"}
        }
    }
}