using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Claims;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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

    [BindProperty(Name = "ReturnUrl", SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    // use me
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

            if (ReturnUrl is not null)
            {
                return Redirect(ReturnUrl);
            }
            else
            {
                return RedirectToPage("./Index");
            }
        }
        else
        {
            InvalidLogin = customerTokenResult.Error ?? "Unidentified error";
            return Page();
        }
    }
}
