using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;

// cross controller tests designed to test user-journey through application

namespace SessionAPI.IntegrationTests;

public class UserExperienceTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public UserExperienceTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact(DisplayName = "Test user journey, register and place order")]
    public async Task User_Can_Register_And_Successfully_Post_And_Retrieve_Sale()
    {
        // Arrange
        var customer = new CreateCustomerDTO
        {
            Name = "Registration Test",
            Email = "regtest@email.com",
            Password = "regtest123",
        };

        Dictionary<Guid, int> BasketQuantitiesByProductId = new Dictionary<Guid, int>();

        var productResponse = await _client.GetAsync($"api/products");

        productResponse.EnsureSuccessStatusCode();
        var testItems = await productResponse.Content.ReadFromJsonAsync<List<Product>>() ?? [];

        BasketQuantitiesByProductId[testItems[0].Id] = 1;

        var saleRequestDTO = new SaleDTO
        {
            QuantitiesByProductID = BasketQuantitiesByProductId,
            Date = DateOnly.FromDateTime(DateTime.Now),
            Time = TimeOnly.FromDateTime(DateTime.Now),
        };

        // Act - Register customer
        var registrationResponse = await _client.PostAsJsonAsync(
            "api/customers/register",
            customer
        );

        registrationResponse.EnsureSuccessStatusCode();

        var registrationResponseJson = await registrationResponse.Content.ReadAsStringAsync();
        var registrationToken =
            JsonSerializer
                .Deserialize<JsonElement>(registrationResponseJson)
                .GetProperty("token")
                .GetString()
            ?? "";

        // Act - Send sale request
        var saleRequest = new HttpRequestMessage(HttpMethod.Post, $"api/sales");
        saleRequest.Headers.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            registrationToken
        );
        var body = saleRequestDTO;
        saleRequest.Content = JsonContent.Create(body);
        var saleResponse = await _client.SendAsync(saleRequest);

        saleResponse.EnsureSuccessStatusCode();

        // Act - get sale id from this
        var saleResponseJsonString = await saleResponse.Content.ReadAsStringAsync();

        var saleId =
            JsonSerializer
                .Deserialize<JsonElement>(saleResponseJsonString)
                .GetProperty("id")
                .GetString()
            ?? "";

        // Assert my-orders contains the new sale
        var mySalesRequest = new HttpRequestMessage(HttpMethod.Get, $"api/sales");
        mySalesRequest.Headers.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            registrationToken
        );

        var mySalesResponse = await _client.SendAsync(mySalesRequest);

        mySalesResponse.EnsureSuccessStatusCode();

        var mysalesList = await mySalesResponse.Content.ReadFromJsonAsync<List<Sale>>() ?? [];

        Assert.Contains(mysalesList, item => item.Id == Guid.Parse(saleId));
    }

    [Fact(DisplayName = "Test user journey, register, login and place order")]
    public async Task User_Can_Register_Login_And_Successfully_Post_And_Retrieve_Sale()
    {
        // Arrange
        var customer = new CreateCustomerDTO
        {
            Name = "Login Test",
            Email = "logintest@email.com",
            Password = "logintest1234",
        };

        var customerLogin = new LoginCustomerDTO
        {
            Email = customer.Email,
            Password = customer.Password,
        };

        Dictionary<Guid, int> BasketQuantitiesByProductId = new Dictionary<Guid, int>();

        var productResponse = await _client.GetAsync($"api/products");

        productResponse.EnsureSuccessStatusCode();
        var testItems = await productResponse.Content.ReadFromJsonAsync<List<Product>>() ?? [];

        BasketQuantitiesByProductId[testItems[0].Id] = 1;

        var saleRequestDTO = new SaleDTO
        {
            QuantitiesByProductID = BasketQuantitiesByProductId,
            Date = DateOnly.FromDateTime(DateTime.Now),
            Time = TimeOnly.FromDateTime(DateTime.Now),
        };

        // Act - Register customer
        var registrationResponse = await _client.PostAsJsonAsync(
            "api/customers/register",
            customer
        );

        registrationResponse.EnsureSuccessStatusCode();

        // Act - Login customer
        var loginResponse = await _client.PostAsJsonAsync("api/customers/login", customerLogin);

        loginResponse.EnsureSuccessStatusCode();

        var loginResponseJson = await loginResponse.Content.ReadAsStringAsync();
        var loginToken =
            JsonSerializer
                .Deserialize<JsonElement>(loginResponseJson)
                .GetProperty("token")
                .GetString()
            ?? "";

        // Act - Send sale request
        var saleRequest = new HttpRequestMessage(HttpMethod.Post, $"api/sales");
        saleRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", loginToken);
        var body = saleRequestDTO;
        saleRequest.Content = JsonContent.Create(body);

        var saleResponse = await _client.SendAsync(saleRequest);

        saleResponse.EnsureSuccessStatusCode();

        // Act - get sale id from this
        var saleResponseJsonString = await saleResponse.Content.ReadAsStringAsync();

        var saleId =
            JsonSerializer
                .Deserialize<JsonElement>(saleResponseJsonString)
                .GetProperty("id")
                .GetString()
            ?? "";

        // Assert my-orders contains the new sale
        var mySalesRequest = new HttpRequestMessage(HttpMethod.Get, $"api/sales");
        mySalesRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", loginToken);

        var mySalesResponse = await _client.SendAsync(mySalesRequest);

        mySalesResponse.EnsureSuccessStatusCode();

        var mysalesList = await mySalesResponse.Content.ReadFromJsonAsync<List<Sale>>() ?? [];

        Assert.Contains(mysalesList, item => item.Id == Guid.Parse(saleId));
    }
}
