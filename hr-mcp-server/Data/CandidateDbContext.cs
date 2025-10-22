using HRMCPServer;
using Microsoft.EntityFrameworkCore;

namespace HRMCPServer.Data;

public class CandidateDbContext : DbContext
{
    public CandidateDbContext(DbContextOptions<CandidateDbContext> options)
        : base(options)
    {
    }

    public DbSet<Candidate> Candidates => Set<Candidate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var candidateEntity = modelBuilder.Entity<Candidate>();

        candidateEntity.HasKey(c => c.Id);

        candidateEntity.Property(c => c.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        candidateEntity.Property(c => c.LastName)
            .IsRequired()
            .HasMaxLength(100);

        candidateEntity.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(256);

        candidateEntity.HasIndex(c => c.Email)
            .IsUnique();

        candidateEntity.Property(c => c.CurrentRole)
            .HasMaxLength(200);

        candidateEntity.Property(c => c.SpokenLanguagesData)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        candidateEntity.Property(c => c.SkillsData)
            .IsRequired()
            .HasColumnType("nvarchar(max)");
    }
}
