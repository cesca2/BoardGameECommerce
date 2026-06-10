using System.Security.Principal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BoardGameCommerce.Pages;

public class CreateCustomerModel : PageModel
{
    private readonly CustomersApiClient _customersApi;

    [BindProperty]
    public string Name { get; set; } = "";

    [BindProperty]
    public string Email { get; set; } = "";

    [BindProperty]
    public string Password { get; set; } = "";
    public bool ValidModelEntry = true;
    public string InvalidRegistration = "";

    // To Do: Retrieve error message from API for this case
    public bool CustomerExists = false;

    public CreateCustomerModel(CustomersApiClient customersApi)
    {
        _customersApi = customersApi;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        InvalidRegistration = "";
        CreateCustomerRequest customer = new CreateCustomerRequest
        {
            Name = Name,
            Email = Email,
            Password = Password,
        };

        if (!ModelState.IsValid)
        {
            ValidModelEntry = false;
            return Page();
        }
        else
        {
            var customerTokenResult = await _customersApi.CreateCustomer(customer);
            if (customerTokenResult.Success)
            {
                HttpContext.Session.SetString("UserToken", customerTokenResult.Response ?? "");

                var customerInfo = await _customersApi.GetCustomer(
                    customerTokenResult.Response ?? ""
                );

                if (customerInfo.Success && customerInfo.Response is not null)
                {
                    HttpContext.Session.SetString("UserName", customerInfo.Response.Name);
                    HttpContext.Session.SetString("UserEmail", customerInfo.Response.Email);
                    HttpContext.Session.SetString("UserId", customerInfo.Response.Id.ToString());
                }
                else
                {
                    InvalidRegistration = customerInfo.Error ?? "Invalid registration request";
                    return Page();
                }
            }
            else
            {
                InvalidRegistration = customerTokenResult.Error ?? "Invalid registration request";
                return Page();
            }
        }

        if (HttpContext.Session.GetInt32("CheckoutRequested") == 1)
        {
            return RedirectToPage("./Checkout");
        }
        else
        {
            return RedirectToPage("./Index");
        }
    }
}
