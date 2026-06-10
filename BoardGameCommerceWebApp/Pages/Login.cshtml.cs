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
    public string Email { get; set; } = "";

    [BindProperty]
    public string Password { get; set; } = "";

    public bool ValidModelEntry = true;
    public string InvalidLogin = "";

    public LoginModel(CustomersApiClient customersApi)
    {
        _customersApi = customersApi;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        InvalidLogin = "";
        var customerTokenResult = await _customersApi.Login(
            new CreateLoginRequest { Email = Email, Password = Password }
        );
        if (customerTokenResult.Success)
        {
            HttpContext.Session.SetString("UserToken", customerTokenResult.Response);

            var customerInfo = await _customersApi.GetCustomer(customerTokenResult.Response);

            if (customerInfo.Success)
            {
                HttpContext.Session.SetString("UserName", customerInfo.Response.Name);
                HttpContext.Session.SetString("UserEmail", customerInfo.Response.Email);
                HttpContext.Session.SetString("UserId", customerInfo.Response.Id.ToString());

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
                InvalidLogin = customerInfo.Error;
                return Page();
            }
        }
        else
        {
            InvalidLogin = customerTokenResult.Error;
            return Page();
        }
    }
}
