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
    public async Task<List<GetSaleResponse>> GetSalesByCustomerId(Guid customer_id)
    {
        HttpResponseMessage response = await _httpClient.GetAsync($"api/Sales");
        if (response.IsSuccessStatusCode){
        var sales = await _httpClient
            .GetFromJsonAsync<List<GetSaleResponse>>($"api/Sales");
        return sales.Where( sale => sale.customer_Id == customer_id ).ToList() ?? [];
        
    }
    else
        {
            return [];
        }
    }
    public async Task<GetSaleResponse?> GetSaleById(Guid? sale_id)
    {
        HttpResponseMessage response = await _httpClient.GetAsync($"api/Sales/{sale_id}");
        if (response.IsSuccessStatusCode){
        var sales = await _httpClient
            .GetFromJsonAsync<GetSaleResponse>($"api/Sales/{sale_id}");
        return sales;
        
    }
    else
        {
            return null;;
        }

}}