using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

public class CustomersApiClient
{
    private readonly HttpClient _httpClient;

    public CustomersApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> CreateCustomer(CreateCustomerRequest customerRequest)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(
            "api/customers/register",
            customerRequest
        );
        var json = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var token = JsonSerializer
                .Deserialize<JsonElement>(json)
                .GetProperty("token")
                .GetString();
            return token ?? "";
        }
        else
        {
            return "";
        }
    }

    public async Task<string> Login(CreateLoginRequest customerRequest)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(
            "api/customers/login",
            customerRequest
        );
        var json = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var token =
                JsonSerializer.Deserialize<JsonElement>(json).GetProperty("token").GetString()
                ?? "";
            return token;
        }
        else
        {
            return "";
        }
    }

    public async Task<GetCustomerResponse?> GetCustomer(string token)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/customers/me");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<GetCustomerResponse>();
        }
        else
        {
            return null;
        }
    }
}
