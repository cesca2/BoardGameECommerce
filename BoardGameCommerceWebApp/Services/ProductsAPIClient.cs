public class ProductsApiClient
{
    private readonly HttpClient _httpClient;

    public ProductsApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Product>> GetProductsAsync()
    {
        var response = await _httpClient.GetAsync("api/Products");

        response.EnsureSuccessStatusCode();

        var products = await response.Content
            .ReadFromJsonAsync<List<GetProductsResponse>>();


        // Return in a parsed format
        return products.Select( product => new Product()
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            YearPublished = product.YearPublished
        }).ToList() ?? [];
    }

    /*
    public async Task<Product?> GetProductAsync(int id)
    {
        return await _httpClient
            .GetFromJsonAsync<Product>($"api/Products/{id}");
    }
    */
}