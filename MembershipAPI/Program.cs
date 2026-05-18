using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MembershipAPI.Repositories;
using MembershipAPI.Repositories.Interfaces;
using MongoDB.Driver;
using NLog;
using NLog.Web;
using Scalar.AspNetCore;
using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.Commons;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings()
    .GetCurrentClassLogger();

logger.Debug("Starting MembershipService");

// Endpoint til vault, vault og Service skal være på samme docker netværk, så 'localhost' bliver til 'vault' i endpoint
var EndPoint = Environment.GetEnvironmentVariable("VAULT_URL") ?? "https://localhost:8201/";
logger.Debug("Connecting to Hashicorp Vault on: {0}", EndPoint);
var httpClientHandler = new HttpClientHandler();
httpClientHandler.ServerCertificateCustomValidationCallback =
    (message, cert, chain, sslPolicyErrors) => { return true; };
    
// Initialize one of the several auth methods.
IAuthMethodInfo authMethod =
    new TokenAuthMethodInfo("00000000-0000-0000-0000-000000000000");
// Initialize settings. You can also set proxies, custom delegates etc. here.
var vaultClientSettings = new VaultClientSettings(EndPoint, authMethod)
{
    Namespace = "",
    MyHttpClientProviderFunc = handler
        => new HttpClient(httpClientHandler) {
            BaseAddress = new Uri(EndPoint)
        }
};
logger.Debug("Getting JWT secret from vault");
IVaultClient vaultClient = new VaultClient(vaultClientSettings);
string jwtSecretString = "";
try
{
    Secret<SecretData> jwtSecret = await vaultClient.V1.Secrets.KeyValue.V2
        .ReadSecretAsync(path: "auth", mountPoint: "secret");
    jwtSecretString = jwtSecret.Data.Data["JWT_SECRET"].ToString();
    if (string.IsNullOrWhiteSpace(jwtSecretString))
        throw new NullReferenceException("JWT_SECRET not found");
    Console.WriteLine(jwtSecretString);
    Environment.SetEnvironmentVariable("JWT_SECRET", jwtSecretString);
    
    Secret<SecretData> mongoSecrets = await vaultClient.V1.Secrets.KeyValue.V2
        .ReadSecretAsync(path: "mongo", mountPoint: "secret");
    string connectionString;
    if (Environment.GetEnvironmentVariable("DOCKER") != null)
    {
        connectionString = mongoSecrets
                               .Data.Data["MONGO_CONNECTION_STRING"]?.ToString()
                           ?? throw new NullReferenceException(
                               "MONGO_CONNECTION_STRING not found in Vault");
    }
    else
    {
        connectionString = "mongodb://admin:secret123@localhost:27017/?authSource=admin";
    }
    Console.WriteLine(connectionString);
    Environment.SetEnvironmentVariable("MONGO_CONNECTION_STRING", connectionString);
    
    string mongoDbName = mongoSecrets.Data.Data["MONGO_MEMBERSHIP_DB"].ToString();
    if (string.IsNullOrWhiteSpace(mongoDbName))
        throw new NullReferenceException("MONGO_DATABASE_NAME not found");
    Console.WriteLine(mongoDbName);
    Environment.SetEnvironmentVariable("MONGO_DATABASE_NAME", mongoDbName);
}
catch (Exception e)
{
    logger.Error($"{e.InnerException.Message}");
    Console.WriteLine("Something went wrong connecting to Vault: " + e.InnerException.Message);
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = false;
        o.TokenValidationParameters = new TokenValidationParameters()
        {
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET"))),
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ClockSkew = TimeSpan.Zero,
        };
    });

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Logging.ClearProviders();
builder.Host.UseNLog();

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var connectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING");

    if (string.IsNullOrWhiteSpace(connectionString))
        throw new InvalidOperationException("MONGO_CONNECTION_STRING environment variable is not set");
    return new MongoClient(connectionString);
});

builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var mongoClient = sp.GetRequiredService<IMongoClient>();
    
    var databaseName = Environment.GetEnvironmentVariable("MONGO_DATABASE_NAME");
    
    if (string.IsNullOrWhiteSpace(databaseName))
        throw new InvalidOperationException("MONGO_DATABASE_NAME environment variable is not set");
    
    return mongoClient.GetDatabase(databaseName);
});

builder.Services.AddScoped<IAddOnRepository, AddOnRepositoryMongoDb>();
builder.Services.AddScoped<IMemberSubscriptionRepository, MemberSubscriptionRepositoryMongoDb>();
builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepositoryMongoDb>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
