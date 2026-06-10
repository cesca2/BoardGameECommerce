using System.Text.Json;

public class ProductsApiClient
{
    private readonly HttpClient _httpClient;

    public ProductsApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ApiResult<List<Product>>> GetProductsAsync(string searchTerm)
    {
        try
        {
            HttpResponseMessage response;
            string? json;
            if (string.IsNullOrEmpty(searchTerm))
            {
                response = await _httpClient.GetAsync($"api/products");
                json = await response.Content.ReadAsStringAsync();
            }
            else
            {
                response = await _httpClient.GetAsync($"api/products?SearchTerm={searchTerm}");

                json = await response.Content.ReadAsStringAsync();
            }

            if (response.IsSuccessStatusCode)
            {
                var products =
                    await response.Content.ReadFromJsonAsync<List<GetProductsResponse>>() ?? [];
                json = await response.Content.ReadAsStringAsync();

                // Return in a parsed format
                return ApiResultFactory<List<Product>>.Ok(
                    products
                        .Select(product => new Product()
                        {
                            Id = product.Id,
                            Name = product.Name,
                            Price = product.Price,
                            YearPublished = product.YearPublished,
                        })
                        .ToList()
                        ?? []
                );
            }
            else
            {
                var error =
                    JsonSerializer.Deserialize<JsonElement>(json).GetProperty("error").GetString()
                    ?? "";
                return ApiResultFactory<List<Product>>.Fail(error);
            }
        }
        catch (HttpRequestException)
        {
            return ApiResultFactory<List<Product>>.Fail("Unable to connect to required service");
        }
    }

    public async Task<ApiResult<Product?>> GetProductAsync(string id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/products");
            var json = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var product = await _httpClient.GetFromJsonAsync<Product>($"api/products/{id}");
                return ApiResultFactory<Product?>.Ok(product);
            }
            else
            {
                var error =
                    JsonSerializer.Deserialize<JsonElement>(json).GetProperty("error").GetString()
                    ?? "";
                return ApiResultFactory<Product?>.Fail(error);
            }
        }
        catch (HttpRequestException)
        {
            return ApiResultFactory<Product?>.Fail("Unable to connect to required service");
        }
    }
}
