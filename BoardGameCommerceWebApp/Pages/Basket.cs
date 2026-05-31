using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BoardGameCommerce.Pages;

public class BasketModel : PageModel
{
    public bool Submitted = false;
    private readonly ProductsApiClient _productsApi;

    [BindProperty]
    public string Basket { get; set; }

    public List<BasketItem> BasketItems { get; set; }

    public class BasketItem
    {
        public string productId { get; set; }

        public int quantity { get; set; }
    }

    public List<Product> Products { get; set; } = [];

    public BasketModel(ProductsApiClient productsApi)
    {
        _productsApi = productsApi;
    }

    public async Task OnPostAsync()
    {
        Console.WriteLine(Basket[0]);
        BasketItems = JsonSerializer.Deserialize<List<BasketItem>>(Basket);

        foreach (var item in BasketItems)
        {
            Console.WriteLine(item.productId);
            var product = await _productsApi.GetProductAsync(item.productId);
            product.Quantity = item.quantity;
            Products.Add(product);
        }
    }
}
