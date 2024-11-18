using ConsoleApp2;
using JustEat.HttpClientInterception;
using System.Net;

namespace ConsoleApp2UnitTests;


[TestClass]
public class AsyncHttpRequesterTests
{
    [TestMethod]
    public async Task GetStringResultsAsync_Success()
    {
        // Arrange
        var timeout = TimeSpan.FromSeconds(1); // Short timeout for test
        var urls = new List<string>
            {
                "https://www.microsoft.com", // Replace with a mock server if needed
                "https://www.google.com"     // Replace with a mock server if needed
            };
        var expectedResults = new Dictionary<string, string>()
            {
                { urls[0], "Some mocked content" },
                { urls[1], "Some other mocked content" }
            };

        var mockMessageHandler = new MockHttpMessageHandler();
        mockMessageHandler.AddResponse(urls[0], "Some mocked content");
        mockMessageHandler.AddResponse(urls[1], "Some other mocked content");

        var client = new HttpClient(mockMessageHandler);
        var requester = new AsyncHttpRequester(client,timeout);

        // Act
        var results = await requester.GetStringResultsAsync(urls);

        // Assert
        CollectionAssert.AreEquivalent(expectedResults, results);
    }

    //[TestMethod]
    //public async Task GetStringResultsAsync_Timeout()
    //{
    //    // Arrange
    //    var timeout = TimeSpan.FromMilliseconds(100); // Very short timeout for test
    //    var urls = new List<string>() { "https://www.slowwebsite.com" }; // Simulate a slow server

    //    var requester = new AsyncHttpRequester(timeout);

    //    // Act
    //    var results = await requester.GetStringResultsAsync(urls);

    //    // Assert
    //    Assert.IsTrue(results.ContainsKey(urls[0]));
    //    StringAssert.Contains(results[urls[0]], $"Request timed out after {timeout.TotalMilliseconds}ms");
    //}

    [TestMethod]
    public void GetStringResultsAsync_UsingJustEatHttpInterceptor()
    {
        // Arrange
        var options = new HttpClientInterceptorOptions();
        var latency = TimeSpan.FromMilliseconds(2000);
        var serviceA = new HttpRequestInterceptionBuilder()
                .ForHttp()
                .ForHost("localhost1")
                .ForPath("5051")
                .ForPath("health")
                .WithInterceptionCallback((_) => Task.Delay(latency))
                .WithJsonContent(new { name = "servicea" , description = "Service A Is Healthy" });
        var serviceB = new HttpRequestInterceptionBuilder()
                        .ForHttp()
                        .ForHost("localhost2")
                        .ForPath("5052")
                        .ForPath("health")
                        .WithJsonContent(new { name = "serviceb", description = "Service B Is Healthy" });
        var serviceC = new HttpRequestInterceptionBuilder()
                        .ForHttp()
                        .ForHost("localhost3")
                        .ForPath("5053")
                        .ForPath("health")
                        .WithStatus(HttpStatusCode.InternalServerError);
                    

        options.Register(serviceA, serviceB,serviceC);
        var httpClient = options.CreateHttpClient();

        // Arrange
        var timeout = TimeSpan.FromSeconds(1);
        var urls = new List<string>() { "http://localhost1:5051/health", "http://localhost2:5052/health",
        "http://localhost3:5053/health"}; // Simulate a non-existent site

        var requester = new AsyncHttpRequester( httpClient, timeout);

        // Act
        var results =  requester.GetStringResultsAsync(urls).GetAwaiter().GetResult();
        Console.WriteLine(results);

        // Assert
        //Assert.IsTrue(results.ContainsKey(urls[0]));
        ///StringAssert.Contains(results[urls[0]], "Error:"); // Check for any error message
    }
}

public class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly Dictionary<string, string> _responses = new Dictionary<string, string>();
    private readonly Dictionary<string, HttpStatusCode> _statusCodes = new Dictionary<string, HttpStatusCode>();
    public Func<HttpResponseMessage, HttpResponseMessage> HttpResponseProducer { get; set; }

    public void AddResponse(string url, string content, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        _responses.Add(url, content);
        _statusCodes.Add(url, statusCode);
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var url = request.RequestUri.ToString();
        if (!_responses.ContainsKey(url))
        {
            throw new Exception("Unexpected URL: " + url);
        }

        var response = new HttpResponseMessage
        {
            Content = new StringContent(_responses[url]),
            StatusCode = _statusCodes[url]
        };

        return await Task.FromResult(response);
    }
}
