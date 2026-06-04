using Microsoft.AspNetCore.Mvc.Testing;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
    where TProgram : class
{
    private readonly string _datasource = Guid.NewGuid().ToString() + ".db";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
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

            var newdbContextDescriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(IDbConnectionFactory)
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
        if (File.Exists(_datasource))
        {
            File.Delete(_datasource);
        }
    }
}
