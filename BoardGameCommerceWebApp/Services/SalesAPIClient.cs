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

    public async Task<ApiResult<Guid>> CreateSale(CreateSaleRequest saleRequest, string token)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"api/sales");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // optional JSON body
            var body = saleRequest;

            request.Content = JsonContent.Create(body);
            var response = await _httpClient.SendAsync(request);

            var jsonString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var id =
                    JsonSerializer
                        .Deserialize<JsonElement>(jsonString)
                        .GetProperty("id")
                        .GetString()
                    ?? "";
                return ApiResultFactory<Guid>.Ok(Guid.Parse(id));
            }
            else
            {
                var error =
                    JsonSerializer
                        .Deserialize<JsonElement>(jsonString)
                        .GetProperty("error")
                        .GetString()
                    ?? "";
                return ApiResultFactory<Guid>.Fail(error);
                ;
            }
        }
        catch (HttpRequestException)
        {
            return ApiResultFactory<Guid>.Fail(
                "Unable to connect to required service to place sale, please try again later."
            );
        }
    }

    public async Task<ApiResult<List<GetSaleResponse>>> GetSales(string token)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/sales");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.SendAsync(request);
            var jsonString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return ApiResultFactory<List<GetSaleResponse>>.Ok(
                    await response.Content.ReadFromJsonAsync<List<GetSaleResponse>>() ?? []
                );
            }
            else
            {
                var error =
                    JsonSerializer
                        .Deserialize<JsonElement>(jsonString)
                        .GetProperty("error")
                        .GetString()
                    ?? "";
                return ApiResultFactory<List<GetSaleResponse>>.Fail(error);
                ;
            }
        }
        catch (HttpRequestException)
        {
            return ApiResultFactory<List<GetSaleResponse>>.Fail(
                "Unable to connect to required service, please try again later."
            );
        }
    }
}
