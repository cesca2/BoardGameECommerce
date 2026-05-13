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
}