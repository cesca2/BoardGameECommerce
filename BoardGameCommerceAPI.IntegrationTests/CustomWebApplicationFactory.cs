using Microsoft.AspNetCore.Mvc.Testing;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
    where TProgram : class
{
    // create unique testing datasource for each factory instance to prevent conflicts between tests
    private readonly string _datasource = Guid.NewGuid().ToString() + ".db";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(
            (context, config) =>
            {
                // override user-secrets in test scenario
                config.AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["AdminInfo:Name"] = "test-admin",
                        ["AdminInfo:Email"] = "test-admin@email.com",
                        ["AdminInfo:Password"] = "testadmin123",
                        ["Jwt:Issuer"] = "test-issuer",
                        ["Jwt:Audience"] = "test-audience",
                        ["Jwt:SecretKey"] = "F96DE71F-A6FB-4B71-843F-2CAA2668A4E0",
                        ["Database:Password"] = "testdb123",
                    }
                );
            }
        );

        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(IDbConnectionFactory)
            );

            if (dbContextDescriptor is not null)
            {
                services.Remove(dbContextDescriptor);
            }

            // Reconfigure datasource in tests
            services.AddSingleton<IDbConnectionFactory, SqliteConnectionFactory>(
                serviceProvider => new SqliteConnectionFactory(
                    config: serviceProvider.GetRequiredService<IConfiguration>(),
                    dataSource: _datasource
                )
            );
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            DeleteDataSourceFile();
        }
    }

    private void DeleteDataSourceFile()
    {
        // dispose of temp test database file
        if (File.Exists(_datasource))
        {
            File.Delete(_datasource);
        }
    }
}
