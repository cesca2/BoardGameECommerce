using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BoardGameCommerce.Pages;

public class CheckoutModel : PageModel
{
    private readonly SalesApiClient _salesApi;
    private readonly ProductsApiClient _productsApi;
    public string CheckoutPageVisitId { get; set; } = "";
    public List<Product> Products { get; set; } = [];

    [BindProperty]
    public string Basket { get; set; } = "";

    public List<BasketItem> BasketItems { get; set; } = [];

    public class BasketItem
    {
        public required string productId { get; set; }

        public required int quantity { get; set; }
    }

    public Dictionary<string, int> BasketQuantitiesByProductId = new Dictionary<string, int>();

    public CheckoutModel(SalesApiClient salesApi, ProductsApiClient productsApi)
    {
        _salesApi = salesApi;
        _productsApi = productsApi;
    }

    public IActionResult OnGet()
    {
        CheckoutPageVisitId = "j" + Guid.NewGuid().ToString();
        HttpContext.Session.SetString("CheckoutPageVisitId", CheckoutPageVisitId);

        var username = HttpContext.Session.GetString("UserName");

        if (string.IsNullOrWhiteSpace(username))
        {
            HttpContext.Session.SetInt32("CheckoutRequested", 1);
            return RedirectToPage("./Login");
        }
        else
        {
            return Page();
        }
    }

    public async Task OnPostBasketAsync()
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

        HttpContext.Session.SetString("BasketProducts", JsonSerializer.Serialize(Products));
    }

    public async Task<IActionResult> OnPostCheckoutAsync()
    {
        var products_json = HttpContext.Session.GetString("BasketProducts") ?? "";
        var products = JsonSerializer.Deserialize<List<Product>>(products_json) ?? [];

        foreach (var product in products)
        {
            BasketQuantitiesByProductId[product.Id.ToString()] = product.Quantity;
        }

        var token = HttpContext.Session.GetString("UserToken") ?? "";

        var sale = new CreateSaleRequest
        {
            quantitiesByProductID = BasketQuantitiesByProductId,
            Date = DateOnly.FromDateTime(DateTime.Now),
            Time = TimeOnly.FromDateTime(DateTime.Now),
        };

        var sale_conf = await _salesApi.CreateSale(sale, token);
        if (!sale_conf.Success)
        {
            HttpContext.Session.SetString(
                "CheckoutError",
                sale_conf.Error ?? "Unable to place order"
            );
            CheckoutPageVisitId += Guid.NewGuid().ToString();
            return Page();
        }
        else
        {
            HttpContext.Session.Remove("CheckoutError");

            return RedirectToPage(
                "./OrderConfirmation",
                new { id = sale_conf.Response.ToString() }
            );
        }
    }
}
