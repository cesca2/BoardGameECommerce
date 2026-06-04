using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;

namespace SessionAPI.IntegrationTests;

public class ProductsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly string ProductsPath = "/api/products";

    public ProductsControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact(DisplayName = "Test API products endpoint returns success")]
    public async Task Get_Products_EndpointsReturnSuccesAndCorrectContentType()
    {
        // Act
        var response = await _client.GetAsync(ProductsPath);

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

    [Fact(DisplayName = "Get product by invalid Id returns not found")]
    public async Task Get_Product_By_Invalid_Id_Returns_NotFound()
    {
        // Arrange - Guid.NewGuid() will not return Guid.Empty so use this as test
        var invalid_guid = Guid.Empty;
        var test_id = invalid_guid.ToString();

        // Act
        var response = await _factory.CreateClient().GetAsync($"{ProductsPath}/{test_id}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory(DisplayName = "Test search term query in products")]
    [InlineData("duel", 4)] // test duel with various combinations of caps/lowercase
    [InlineData("Duel", 4)]
    [InlineData("DUEL", 4)]
    [InlineData("DUel", 4)]
    [InlineData("djfld", 0)] // no results expected
    [InlineData("Dice", 8)]
    public async Task Get_Products_With_SearchTerm_Pagination_Returns_CorrectNumberOfProducts(
        string searchTerm,
        int expectedCount
    )
    {
        // Arrange
        var url = $"{ProductsPath}?SearchTerm={searchTerm}";
        int items_count;

        // Act
        var items = await _client.GetFromJsonAsync<List<Product>>(url);

        if (items is not null)
        {
            items_count = items.Count;
        }
        else
        {
            items_count = -1;
        }

        // Assert
        Assert.Equal(expectedCount, items_count);
    }
}
