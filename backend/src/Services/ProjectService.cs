using Microsoft.EntityFrameworkCore;
using TaskDeck.Api.Data;
using TaskDeck.Api.Models;

namespace TaskDeck.Api.Services;

/// <summary>
/// Service for project operations
/// </summary>
public class ProjectService
{
    private readonly AppDbContext _db;

    public ProjectService(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Get all projects for a user (owned + member)
    /// </summary>
    public async Task<List<ProjectDto>> GetProjectsAsync(Guid userId)
    {
        // Get projects where user is owner
        var ownedProjects = await _db.Projects
            .Include(p => p.Owner)
            .Include(p => p.Tasks)
            .Where(p => p.OwnerId == userId && !p.IsArchived)
            .ToListAsync();

        // Get projects where user is a member
        var memberProjects = await _db.ProjectMembers
            .Include(pm => pm.Project)
                .ThenInclude(p => p.Owner)
            .Include(pm => pm.Project)
                .ThenInclude(p => p.Tasks)
            .Where(pm => pm.UserId == userId && !pm.Project.IsArchived)
            .Select(pm => pm.Project)
            .ToListAsync();

        // Combine and order by CreatedAt
        var allProjects = ownedProjects
            .Concat(memberProjects)
            .DistinctBy(p => p.Id)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Color = p.Color,
                Icon = p.Icon,
                IsArchived = p.IsArchived,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                IsOwner = p.OwnerId == userId,
                Owner = new UserDto
                {
                    Id = p.Owner.Id,
                    Email = p.Owner.Email,
                    DisplayName = p.Owner.DisplayName,
                    AvatarUrl = p.Owner.AvatarUrl
                },
                TaskCount = p.Tasks.Count
            })
            .ToList();

        return allProjects;
    }

    /// <summary>
    /// Get a project by ID (accessible by owner or members)
    /// </summary>
    public async Task<ProjectDto?> GetProjectByIdAsync(Guid id, Guid userId)
    {
        var project = await _db.Projects
            .Include(p => p.Owner)
            .Include(p => p.Tasks)
            .Include(p => p.Members)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project == null) return null;

        // Check if user is owner or member
        bool isOwner = project.OwnerId == userId;
        bool isMember = project.Members.Any(m => m.UserId == userId);
        if (!isOwner && !isMember) return null;

        return new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            Color = project.Color,
            Icon = project.Icon,
            IsArchived = project.IsArchived,
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt,
            IsOwner = isOwner,
            Owner = new UserDto
            {
                Id = project.Owner.Id,
                Email = project.Owner.Email,
                DisplayName = project.Owner.DisplayName,
                AvatarUrl = project.Owner.AvatarUrl
            },
            TaskCount = project.Tasks.Count
        };
    }

    /// <summary>
    /// Create a new project
    /// </summary>
    public async Task<ProjectDto> CreateProjectAsync(CreateProjectDto dto, Guid userId)
    {
        var owner = await _db.Users.FindAsync(userId);
        if (owner == null) throw new InvalidOperationException("User not found");

        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            Color = dto.Color,
            Icon = dto.Icon,
            OwnerId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _db.Projects.Add(project);
        await _db.SaveChangesAsync();

        return new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            Color = project.Color,
            Icon = project.Icon,
            IsArchived = project.IsArchived,
            CreatedAt = project.CreatedAt,
            Owner = new UserDto
            {
                Id = owner.Id,
                Email = owner.Email,
                DisplayName = owner.DisplayName,
                AvatarUrl = owner.AvatarUrl
            },
            TaskCount = 0
        };
    }

    /// <summary>
    /// Update an existing project
    /// </summary>
    public async Task<ProjectDto?> UpdateProjectAsync(Guid id, UpdateProjectDto dto, Guid userId)
    {
        var project = await _db.Projects
            .Include(p => p.Owner)
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == id && p.OwnerId == userId);

        if (project == null) return null;

        if (dto.Name != null) project.Name = dto.Name;
        if (dto.Description != null) project.Description = dto.Description;
        if (dto.Color != null) project.Color = dto.Color;
        if (dto.Icon != null) project.Icon = dto.Icon;
        if (dto.IsArchived.HasValue) project.IsArchived = dto.IsArchived.Value;
        project.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            Color = project.Color,
            Icon = project.Icon,
            IsArchived = project.IsArchived,
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt,
            Owner = new UserDto
            {
                Id = project.Owner.Id,
                Email = project.Owner.Email,
                DisplayName = project.Owner.DisplayName,
                AvatarUrl = project.Owner.AvatarUrl
            },
            TaskCount = project.Tasks.Count
        };
    }

    /// <summary>
    /// Delete a project
    /// </summary>
    public async Task<bool> DeleteProjectAsync(Guid id, Guid userId)
    {
        var project = await _db.Projects.FirstOrDefaultAsync(p => p.Id == id && p.OwnerId == userId);
        if (project == null) return false;

        _db.Projects.Remove(project);
        await _db.SaveChangesAsync();
        return true;
    }
}
