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

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/Customers/register", customerRequest);
        var json = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var token = JsonSerializer.Deserialize<JsonElement>(json)
    .GetProperty("token")
    .GetString();
            Console.WriteLine("success");
            return token;
        }
        else
        {
            return "";
        }


    }
    public async Task<string> Login(CreateLoginRequest customerRequest)
    {

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/Customers/login", customerRequest);
        var json = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("success");
            var token = JsonSerializer.Deserialize<JsonElement>(json)
                    .GetProperty("token")
                    .GetString();
            Console.WriteLine(token);
            return token;
        }
        else
        {
            return "";
        }


    }
    public async Task<GetCustomerResponse?> GetCustomer(string token)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/Customers/me");
        request.Headers.Authorization =
             new AuthenticationHeaderValue("Bearer", token);
        var response = await _httpClient.SendAsync(request);
        Console.WriteLine(request.Headers.Authorization?.ToString());

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync
                <GetCustomerResponse>();
        }
        else
        {
            Console.WriteLine($"Status code: {(int)response.StatusCode}");
            return null;
        }

    }
}