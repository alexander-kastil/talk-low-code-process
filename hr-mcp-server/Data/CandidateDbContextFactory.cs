using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace HRMCPServer.Data;

public class CandidateDbContextFactory : IDesignTimeDbContextFactory<CandidateDbContext>
{
    public CandidateDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("CandidateDatabase")
            ?? throw new InvalidOperationException("Connection string 'CandidateDatabase' not found.");

        var optionsBuilder = new DbContextOptionsBuilder<CandidateDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new CandidateDbContext(optionsBuilder.Options);
    }
}
