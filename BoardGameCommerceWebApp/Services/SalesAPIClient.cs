using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

public class SalesApiClient
{
    private readonly HttpClient _httpClient;

    public SalesApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Guid> CreateSale(CreateSaleRequest saleRequest, string token)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/sales");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // optional JSON body
        var body = saleRequest;

        request.Content = JsonContent.Create(body);
        var response = await _httpClient.SendAsync(request);

        var jsonString = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();
        if (response.IsSuccessStatusCode)
        {
            return Guid.Parse(
                JsonSerializer.Deserialize<JsonElement>(jsonString).GetProperty("id").GetString()
                    ?? ""
            );
        }
        else
        {
            return Guid.Empty;
        }
    }

    public async Task<List<GetSaleResponse>> GetSales(string token)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/sales");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<GetSaleResponse>>() ?? [];
        }
        else
        {
            return [];
        }
    }
}
