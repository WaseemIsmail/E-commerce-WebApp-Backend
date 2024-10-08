

using EcomWave.Configurations;
using EcomWave.Repositories;
using EcomWave.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Set Kestrel server options for custom ports
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    // Listen to HTTP requests on port 5000
    serverOptions.ListenAnyIP(5000);
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// add cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000")
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
});

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

builder.Services.AddScoped<InventoryRepository>();
builder.Services.AddScoped<InventoryService>();

builder.Services.AddScoped<OrderRepository>();
builder.Services.AddScoped<OrderService>();


// JWT Settings from appsettings.json

var jwtSettings = builder.Configuration.GetSection("Jwt");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
    };
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

// Add authentication before authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
