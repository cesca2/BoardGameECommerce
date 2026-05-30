using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BoardGameCommerce.Pages;

public class ProductModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? Query { get; set; }

    private readonly ProductsApiClient _productsApi;

    public List<Product> Products { get; set; } = [];

    public ProductModel(ProductsApiClient productsApi)
    {
        _productsApi = productsApi;
    }

    public async Task OnGetAsync()

    {

        Products = await _productsApi.GetProductsAsync(Query);

    }
}