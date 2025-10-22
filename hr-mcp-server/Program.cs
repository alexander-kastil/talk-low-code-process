using HRMCPServer.Data;
using HRMCPServer.Services;
using Microsoft.EntityFrameworkCore;
using ModelContextProtocol.Server;

var builder = WebApplication.CreateBuilder(args);

// Configure SQL Server-backed candidate database
builder.Services.AddDbContext<CandidateDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CandidateDatabase")));

// Register the candidate service
builder.Services.AddScoped<ICandidateService, CandidateService>();

// Add the MCP services: the transport to use (HTTP) and the tools to register.
builder.Services.AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly();

var app = builder.Build();

// Ensure database is created and seeded
await CandidateDbInitializer.InitializeAsync(app.Services);

// Configure the application to use the MCP server
app.MapMcp();

// Run the application
// This will start the MCP server and listen for incoming requests.
app.Run();
