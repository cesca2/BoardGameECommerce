using System.Net;
using System.Net.Http.Headers;

public class SalesApiClient
{
    private readonly HttpClient _httpClient;

    public SalesApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> CreateSale(CreateSaleRequest saleRequest, string token)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/Sales");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // optional JSON body
        var body = saleRequest;

        request.Content = JsonContent.Create(body);
        var response = await _httpClient.SendAsync(request);

        var jsonString = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();
        if (response.IsSuccessStatusCode)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<List<GetSaleResponse>> GetSales(string token)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/Sales");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<GetSaleResponse>>();
        }
        else
        {
            return null;
        }
    }
}
