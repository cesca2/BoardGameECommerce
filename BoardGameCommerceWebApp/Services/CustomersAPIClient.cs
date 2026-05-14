using System.Net;

public class CustomersApiClient
{
    private readonly HttpClient _httpClient;

    public CustomersApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> CreateCustomer(CreateCustomerRequest customerRequest)
    {

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync( "api/Customers", customerRequest);
        var jsonString = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("success");
            return true;

        }
        else
        {
            return false;
        }


    }

    public async Task<GetCustomerResponse?> GetCustomerByEmail(string email)
    {
        HttpResponseMessage response = await _httpClient.GetAsync($"api/Customers/lookup/{email}");
        if (response.IsSuccessStatusCode){
        return await _httpClient
            .GetFromJsonAsync<GetCustomerResponse>($"api/Customers/lookup/{email}");
    }
    else
        {
            return null;
        }
    
}}