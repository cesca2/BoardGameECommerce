using System.Net;
using System.Runtime.InteropServices.Marshalling;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BoardGameCommerce.Pages;

public class LoginModel : PageModel
{
    private readonly CustomersApiClient _customersApi;

    [BindProperty]
    public string Email { get; set; }

    [BindProperty]
    public string Password { get; set; }

    public bool ValidModelEntry = true;

    public LoginModel(CustomersApiClient customersApi)
    {
        _customersApi = customersApi;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        string customerToken = await _customersApi.Login(
            new CreateLoginRequest { Email = Email, Password = Password }
        );

        if (!string.IsNullOrEmpty(customerToken))
        {
            GetCustomerResponse customerInfo = await _customersApi.GetCustomer(customerToken);
            HttpContext.Session.SetString("UserToken", customerToken);

            // To Do - refactor to only store UserName which is needed for front-end display
            HttpContext.Session.SetString("UserName", customerInfo.Name);
            HttpContext.Session.SetString("UserEmail", customerInfo.Email);
            HttpContext.Session.SetString("UserId", customerInfo.Id.ToString());

            if (HttpContext.Session.GetInt32("CheckoutRequested") == 1)
            {
                return RedirectToPage("./Checkout");
            }
            else
            {
                return RedirectToPage("./Index");
            }
        }
        else
        {
            ValidModelEntry = false;

            return Page();
        }
    }
}
