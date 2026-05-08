var builder = WebApplication.CreateBuilder(args);



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// Register configuration-based services
builder.Services.AddSingleton<IDbConnectionFactory, SqliteConnectionFactory>(serviceProvider => new SqliteConnectionFactory(
            config: serviceProvider.GetRequiredService<IConfiguration>(),
            dataSource: "Default"));

builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

// 🔧 Initialize the database at startup
using (var scope = app.Services.CreateScope())
{
    var factory = scope.ServiceProvider.GetRequiredService<IDbConnectionFactory>();

    using var connection = factory.CreateConnection();
    connection.Open();

    DbInitializer.Initialize(connection);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();