using HRMCPServer.Data;
using Microsoft.EntityFrameworkCore;

namespace HRMCPServer.Services;

/// <summary>
/// Service for managing candidate data in memory
/// </summary>
public class CandidateService : ICandidateService
{
    private readonly CandidateDbContext _dbContext;
    private readonly ILogger<CandidateService> _logger;

    public CandidateService(
        CandidateDbContext dbContext,
        ILogger<CandidateService> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<List<Candidate>> GetAllCandidatesAsync()
    {
        return await _dbContext.Candidates
            .AsNoTracking()
            .OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName)
            .ToListAsync();
    }

    public async Task<bool> AddCandidateAsync(Candidate candidate)
    {
        if (candidate == null)
            throw new ArgumentNullException(nameof(candidate));

        var email = candidate.Email.Trim();

        if (await _dbContext.Candidates.AnyAsync(c => c.Email == email))
        {
            return false;
        }

        candidate.Email = email;

        await _dbContext.Candidates.AddAsync(candidate);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Added new candidate: {FullName} ({Email})", candidate.FullName, candidate.Email);
        return true;
    }

    public async Task<bool> UpdateCandidateAsync(string email, Action<Candidate> updateAction)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be null or empty", nameof(email));

        if (updateAction == null)
            throw new ArgumentNullException(nameof(updateAction));

        var normalizedEmail = email.Trim();

        var candidate = await _dbContext.Candidates
            .FirstOrDefaultAsync(c => c.Email == normalizedEmail);

        if (candidate == null)
        {
            return false;
        }

        updateAction(candidate);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Updated candidate with email: {Email}", normalizedEmail);
        return true;
    }

    public async Task<bool> RemoveCandidateAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be null or empty", nameof(email));

        var normalizedEmail = email.Trim();

        var candidate = await _dbContext.Candidates
            .FirstOrDefaultAsync(c => c.Email == normalizedEmail);

        if (candidate == null)
        {
            return false;
        }

        _dbContext.Candidates.Remove(candidate);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Removed candidate with email: {Email}", normalizedEmail);
        return true;
    }

    public async Task<List<Candidate>> SearchCandidatesAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetAllCandidatesAsync();
        }

        var searchTermLower = searchTerm.Trim().ToLowerInvariant();

        var candidates = await _dbContext.Candidates
            .AsNoTracking()
            .ToListAsync();

        var matchingCandidates = candidates.Where(c =>
            c.FirstName.ToLowerInvariant().Contains(searchTermLower) ||
            c.LastName.ToLowerInvariant().Contains(searchTermLower) ||
            c.Email.ToLowerInvariant().Contains(searchTermLower) ||
            c.CurrentRole.ToLowerInvariant().Contains(searchTermLower) ||
            c.Skills.Any(skill => skill.ToLowerInvariant().Contains(searchTermLower)) ||
            c.SpokenLanguages.Any(lang => lang.ToLowerInvariant().Contains(searchTermLower))
        ).ToList();

        return matchingCandidates;
    }
}
