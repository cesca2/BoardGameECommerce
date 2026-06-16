using System.Net.Http.Headers;
using System.Text.Json;

public class DashboardApiClient
{
    private readonly HttpClient _httpClient;

    public DashboardApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ApiResult<GetDashboardResponse?>> GetDashboardSummaryAsync(string token)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/dashboard");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);

            var jsonString = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var summary = await response.Content.ReadFromJsonAsync<GetDashboardResponse>();
                return ApiResultFactory<GetDashboardResponse?>.Ok(summary);
            }
            else
            {
                var error =
                    JsonSerializer
                        .Deserialize<JsonElement>(jsonString)
                        .GetProperty("error")
                        .GetString()
                    ?? "";
                return ApiResultFactory<GetDashboardResponse?>.Fail(error);
            }
        }
        catch (HttpRequestException)
        {
            return ApiResultFactory<GetDashboardResponse?>.Fail(
                "Unable to connect to required service"
            );
        }
    }
}
