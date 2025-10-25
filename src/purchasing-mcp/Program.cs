using Azure;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Server;
using PurchasingService.Configuration;
using PurchasingService.Data;
using PurchasingService.Graph;
using PurchasingService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Prefer a cryptographically-strong provider for less-predictable random values.
builder.Services.AddSingleton<IRandomProvider, SecureRandomProvider>();
// Register ConfigurationService for loading settings from the database
builder.Services.AddScoped<IConfigurationService, ConfigurationService>();
builder.Services.Configure<GraphOptions>(builder.Configuration.GetSection("Graph"));
builder.Services.Configure<AzureOpenAIOptions>(builder.Configuration.GetSection("AzureOpenAI"));
builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection("ConnectionStrings"));
builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection("Email"));

// Configure Entity Framework
var connectionString = builder.Configuration.GetConnectionString("DefaultDatabase");
builder.Services.AddDbContext<PurchasingDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddSingleton<GraphHelper>();
builder.Services.AddSingleton<IChatClient>(sp =>
{
    var aiOptions = sp.GetRequiredService<IOptions<AzureOpenAIOptions>>().Value;

    var client = new AzureOpenAIClient(
        new Uri(aiOptions.Endpoint),
        new AzureKeyCredential(aiOptions.ApiKey));

    var chatClient = client.GetChatClient(aiOptions.Model);

    return chatClient.AsIChatClient();
});

// Register the IOfferRandomizer as a scoped service that loads options from database
builder.Services.AddScoped<IOfferRandomizer, OfferRandomizer>();

// Register purchasing services
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IInquiryService, InquiryService>();
builder.Services.AddScoped<IOrderService, OrderService>();

// Add the MCP services: the transport to use (HTTP) and the tools to register.
builder.Services.AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly();

var app = builder.Build();

// Apply migrations and seed data on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PurchasingDbContext>();
    dbContext.Database.Migrate();
    await DbSeeder.SeedAsync(dbContext);
}

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Purchasing Service v1");
    options.RoutePrefix = string.Empty;
});

app.MapControllers();

// Configure the application to use the MCP server
app.MapMcp();

app.Run();

// Make the implicit Program class public for testing
public partial class Program { }
