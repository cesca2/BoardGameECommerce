using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace BoardGameCommerce.Pages;

public class CheckoutModel : PageModel
{
    private readonly SalesApiClient _salesApi;
    private readonly ProductsApiClient _productsApi;
    public List<Product> Products { get; set; } = [];

    [BindProperty]
    public string Basket { get; set; }

    public List<BasketItem> BasketItems { get; set; }
    public class BasketItem
    {

        public string productId { get; set; }

        public int quantity { get; set; }
    }

    public Dictionary<string, int> BasketQuantitiesByProductId = new Dictionary<string, int>();


    public CheckoutModel(SalesApiClient salesApi, ProductsApiClient productsApi)
    {
        _salesApi = salesApi;
        _productsApi = productsApi;
    }

    public IActionResult OnGet()
    {
        var username = HttpContext.Session.GetString("UserName");
        Console.WriteLine(username);

        if (string.IsNullOrWhiteSpace(username))
        {
            HttpContext.Session.SetInt32("CheckoutRequested", 1);
            return RedirectToPage("./Login");
        }

        else { return Page(); }
    }


    public async Task OnPostBasketAsync()
    {

        BasketItems =
            JsonSerializer.Deserialize<List<BasketItem>>(Basket);     

        foreach (var item in BasketItems)
        {
            Console.WriteLine(item.productId);
            var product = await _productsApi.GetProductAsync(item.productId);
            product.Quantity = item.quantity;
            Products.Add(product);

        }

        HttpContext.Session.SetString(
        "BasketProducts",
        JsonSerializer.Serialize(Products)
);


    }
    public async Task<IActionResult> OnPostCheckoutAsync()
    {

        var customerId = HttpContext.Session.GetString("UserId");

        var products_json = HttpContext.Session.GetString("BasketProducts");
        var products =
            JsonSerializer.Deserialize<List<Product>>(products_json);

        foreach (var product in products)
        {
            BasketQuantitiesByProductId[product.Id.ToString()] = product.Quantity;


        }
        Console.WriteLine(BasketQuantitiesByProductId);

        var token = HttpContext.Session.GetString("UserToken");

        var sale = new CreateSaleRequest { quantitiesByProductID = BasketQuantitiesByProductId, Date = DateOnly.FromDateTime(DateTime.Now), Time = TimeOnly.FromDateTime(DateTime.Now) };

        await _salesApi.CreateSale(sale, token);

        return RedirectToPage("./Index");
    }
}