using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BoardGameCommerce.Pages;

[Authorize(Roles = "Admin")]
public class AdminModel : PageModel
{
    private readonly DashboardApiClient _dashboardApi;
    public GetDashboardResponse? Dashboard;
    public string InvalidPage = "";

    public AdminModel(DashboardApiClient dashboardApi)
    {
        _dashboardApi = dashboardApi;
    }

    public async Task OnGetAsync()
    {
        var token = await HttpContext.GetTokenAsync("api_token") ?? "";

        var result = await _dashboardApi.GetDashboardSummaryAsync(token);
        if (result.Success)
        {
            Dashboard = result.Response;
        }
        else
        {
            InvalidPage = result.Error ?? "Unidentified error";
        }
    }
}
