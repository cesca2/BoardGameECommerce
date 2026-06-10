using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;

public class CustomersApiClient
{
    private readonly HttpClient _httpClient;

    public CustomersApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ApiResult<string>> CreateCustomer(CreateCustomerRequest customerRequest)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(
                "api/customers/register",
                customerRequest
            );
            var json = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var token =
                    JsonSerializer.Deserialize<JsonElement>(json).GetProperty("token").GetString()
                    ?? "";
                return ApiResultFactory<string>.Ok(token);
            }
            else
            {
                var error =
                    JsonSerializer.Deserialize<JsonElement>(json).GetProperty("error").GetString()
                    ?? "";
                return ApiResultFactory<string>.Fail(error);
            }
        }
        catch (HttpRequestException)
        {
            return ApiResultFactory<string>.Fail(
                "Unable to connect to required service, please try again later."
            );
        }
    }

    public async Task<ApiResult<string>> Login(CreateLoginRequest customerRequest)
    {
        try
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
                return ApiResultFactory<string>.Ok(token);
            }
            else
            {
                var error =
                    JsonSerializer.Deserialize<JsonElement>(json).GetProperty("error").GetString()
                    ?? "";
                return ApiResultFactory<string>.Fail(error);
            }
        }
        catch (HttpRequestException)
        {
            return ApiResultFactory<string>.Fail(
                "Unable to connect to required service, please try again later."
            );
        }
    }

    public async Task<ApiResult<GetCustomerResponse?>> GetCustomer(string token)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/customers/me");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return ApiResultFactory<GetCustomerResponse?>.Ok(
                    await response.Content.ReadFromJsonAsync<GetCustomerResponse>()
                );
            }
            else
            {
                var error =
                    JsonSerializer.Deserialize<JsonElement>(json).GetProperty("error").GetString()
                    ?? "";
                return ApiResultFactory<GetCustomerResponse?>.Fail(error);
            }
        }
        catch (HttpRequestException)
        {
            return ApiResultFactory<GetCustomerResponse?>.Fail(
                "Unable to connect to required service, please try again later."
            );
        }
    }
}
