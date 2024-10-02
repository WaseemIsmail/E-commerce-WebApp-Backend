using EcomWave.Configurations;
using EcomWave.Repositories;
using EcomWave.Services;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Set Kestrel server options for custom ports
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    // Listen to HTTP requests on port 5000
    serverOptions.ListenAnyIP(5000);

    // HTTPS requests on port 5001
    // serverOptions.ListenAnyIP(5001, listenOptions => listenOptions.UseHttps());
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Read MongoDB settings from appsettings.json
var mongoDbConfig = builder.Configuration.GetSection("MongoDbConfig").Get<MongoDbConfig>();

if (mongoDbConfig == null)
{
    throw new InvalidOperationException("MongoDB configuration is missing.");
}

// Register MongoDB configuration as a singleton
builder.Services.AddSingleton(mongoDbConfig);

// Register MongoDB client
builder.Services.AddSingleton<IMongoClient>(new MongoClient(mongoDbConfig.ConnectionString));

// Register MongoDB context
builder.Services.AddSingleton<MongoDbContext>();

// Register repositories and services
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<UserService>();

builder.Services.AddScoped<ProductRepository>();
builder.Services.AddScoped<ProductService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// listening on an HTTPS port
// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
