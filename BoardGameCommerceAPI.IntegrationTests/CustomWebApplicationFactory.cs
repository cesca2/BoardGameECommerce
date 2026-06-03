using Microsoft.AspNetCore.Mvc.Testing;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
    where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(IDbConnectionFactory)
            );

            services.Remove(dbContextDescriptor);

            // Reconfigure datasource in tests
            services.AddSingleton<IDbConnectionFactory, SqliteConnectionFactory>(
                serviceProvider => new SqliteConnectionFactory(
                    config: serviceProvider.GetRequiredService<IConfiguration>(),
                    dataSource: "Testing"
                )
            );
        });
    }
}
