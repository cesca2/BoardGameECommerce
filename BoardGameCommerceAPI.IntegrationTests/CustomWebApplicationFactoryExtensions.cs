using System.Net.Http.Headers;

public static class CustomWebApplicationFactoryExtensions
{
    public static HttpClient CreateAuthenticatedClient(
        this CustomWebApplicationFactory<Program> factory,
        string role
    )
    {
        var client = factory.CreateClient();

        var config = factory.Services.GetRequiredService<IConfiguration>();

        string token = JWTTokenFactory.GenerateUserJWT(
            config,
            new Customer { Name = "example", Email = "example@email.com" },
            role
        );

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return client;
    }
}
