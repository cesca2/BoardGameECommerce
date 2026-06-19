using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(customerTokenResult.Response);

                var claims = jwt.Claims.ToList();

                var identity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme
                );

                var authProperties = new AuthenticationProperties();

                authProperties.StoreTokens(
                    new[]
                    {
                        new AuthenticationToken
                        {
                            Name = "api_token",
                            Value = customerTokenResult.Response ?? "",
                        },
                    }
                );
                // var token = await HttpContext.GetTokenAsync("api_token");

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity),
                    authProperties
                );
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
