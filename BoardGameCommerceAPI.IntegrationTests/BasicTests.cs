using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;

namespace SessionAPI.IntegrationTests;

public class BasicTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public BasicTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Theory(DisplayName = "Test API products endpoint returns success")]
    [InlineData("api/products")]
    public async Task Get_EndpointsReturnSuccesAndCorrectContentType(string url)
    {
        // Act

        var response = await _client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        if (response.Content.Headers.ContentType is not null)
        {
            Assert.Equal(
                "application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString()
            );
        }
        else
        {
            Assert.Fail("Content type is empty");
        }
    }
}
