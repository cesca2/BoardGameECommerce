using System.Net;

public class SalesApiClient
{
    private readonly HttpClient _httpClient;

    public SalesApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> CreateSale(CreateSaleRequest saleRequest)
    {

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync( "api/Sales", saleRequest);
        var jsonString = await response.Content.ReadAsStringAsync();
        Console.WriteLine("LOOOK"+jsonString);
        response.EnsureSuccessStatusCode();
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