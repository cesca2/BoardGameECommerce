public class ProductsApiClient
{
    private readonly HttpClient _httpClient;

    public ProductsApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Product>> GetProductsAsync(string searchTerm)
    {
        HttpResponseMessage response;
        if (string.IsNullOrEmpty(searchTerm)){
            response = await _httpClient.GetAsync($"api/Products");
        }   
        else
        {
            response = await _httpClient.GetAsync($"api/Products?SearchTerm={searchTerm}");

        }
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

    
    public async Task<Product?> GetProductAsync(string id)
    {
        return await _httpClient
            .GetFromJsonAsync<Product>($"api/Products/{id}");
    }
    
}