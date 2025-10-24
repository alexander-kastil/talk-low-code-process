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
// Bind configuration options
builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection("ConnectionStrings"));
builder.Services.Configure<OfferRandomizerOptions>(builder.Configuration.GetSection("OfferRandomizer"));
builder.Services.Configure<GraphOptions>(builder.Configuration.GetSection("Graph"));
builder.Services.Configure<AzureOpenAIOptions>(builder.Configuration.GetSection("AzureOpenAI"));

// Configure Entity Framework
var databaseOptions = builder.Configuration.GetSection("ConnectionStrings").Get<DatabaseOptions>();
builder.Services.AddDbContext<PurchasingDbContext>(options =>
    options.UseSqlServer(databaseOptions?.DefaultDatabase ?? throw new InvalidOperationException("DefaultDatabase connection string not configured")));
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

// Register the IOfferRandomizer as a singleton that resolves the configured options
builder.Services.AddSingleton<IOfferRandomizer>(sp =>
{
    var options = sp.GetRequiredService<IOptions<OfferRandomizerOptions>>().Value;
    var random = sp.GetRequiredService<IRandomProvider>();
    return new OfferRandomizer(random, options);
});

// Register purchasing services
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IInquiryService, InquiryService>();
builder.Services.AddScoped<IOrderService, OrderService>();

// Add the MCP services: the transport to use (HTTP) and the tools to register.
builder.Services.AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly();

var app = builder.Build();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PurchasingDbContext>();
    dbContext.Database.EnsureCreated();
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
