using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BoardGameCommerce.Pages;

public class BasketModel : PageModel
{
    private readonly ProductsApiClient _productsApi;

    [BindProperty]
    public string Basket { get; set; } = "";

    public string BasketPageVisitId { get; set; } = "";

    public List<BasketItem> BasketItems { get; set; } = [];

    public class BasketItem
    {
        public required string productId { get; set; }

        public required int quantity { get; set; }
    }

    public string InvalidPage { get; set; } = "";
    public List<Product> Products { get; set; } = [];

    public BasketModel(ProductsApiClient productsApi)
    {
        _productsApi = productsApi;
    }

    public void OnGet()
    {
        BasketPageVisitId = "j" + Guid.NewGuid().ToString();
        HttpContext.Session.SetString("BasketPageVisitId", BasketPageVisitId);
    }

    public async Task OnPostAsync()
    {
        BasketItems = JsonSerializer.Deserialize<List<BasketItem>>(Basket) ?? [];

        foreach (var item in BasketItems)
        {
            var productresponse = await _productsApi.GetProductAsync(item.productId);
            var product = productresponse.Response;
            if (productresponse.Success && product is not null)
            {
                product.Quantity = item.quantity;
                Products.Add(product);
            }
        }
        if (BasketItems.Count > Products.Count)
        {
            InvalidPage = "Unable to load your basket, please try again later.";
        }
    }
}
