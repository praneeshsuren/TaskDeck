using Microsoft.EntityFrameworkCore;
using TaskDeck.Api.Data;
using TaskDeck.Api.Models;

namespace TaskDeck.Api.Services;

/// <summary>
/// Service for managing project invitations
/// </summary>
public class InvitationService
{
    private readonly AppDbContext _db;

    public InvitationService(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Send an invitation to a user by email
    /// </summary>
    public async Task<InvitationDto?> SendInvitationAsync(Guid projectId, string email, Guid invitedByUserId)
    {
        // Check if project exists and user has permission (owner or admin)
        var project = await _db.Projects
            .Include(p => p.Members)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        if (project == null) return null;

        // Check if inviter is owner or admin
        bool isOwner = project.OwnerId == invitedByUserId;
        bool isAdmin = project.Members.Any(m => m.UserId == invitedByUserId && m.Role == ProjectRole.Admin);
        if (!isOwner && !isAdmin)
        {
            throw new InvalidOperationException("You don't have permission to invite users to this project");
        }

        // Find user by email
        var invitedUser = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (invitedUser == null)
        {
            throw new InvalidOperationException("User with this email not found");
        }

        // Check if user is already owner
        if (project.OwnerId == invitedUser.Id)
        {
            throw new InvalidOperationException("User is the project owner");
        }

        // Check if user is already a member
        var existingMember = await _db.ProjectMembers
            .FirstOrDefaultAsync(pm => pm.ProjectId == projectId && pm.UserId == invitedUser.Id);
        if (existingMember != null)
        {
            throw new InvalidOperationException("User is already a member of this project");
        }

        // Check for existing pending invitation
        var existingInvitation = await _db.Invitations
            .FirstOrDefaultAsync(i => i.ProjectId == projectId && i.InvitedUserId == invitedUser.Id && i.Status == InvitationStatus.Pending);
        if (existingInvitation != null)
        {
            throw new InvalidOperationException("User already has a pending invitation");
        }

        var invitedBy = await _db.Users.FindAsync(invitedByUserId);

        var invitation = new Invitation
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            InvitedUserId = invitedUser.Id,
            InvitedByUserId = invitedByUserId,
            Status = InvitationStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _db.Invitations.Add(invitation);
        await _db.SaveChangesAsync();

        return new InvitationDto
        {
            Id = invitation.Id,
            ProjectId = projectId,
            ProjectName = project.Name,
            ProjectColor = project.Color,
            InvitedUser = new UserDto
            {
                Id = invitedUser.Id,
                Email = invitedUser.Email,
                DisplayName = invitedUser.DisplayName,
                AvatarUrl = invitedUser.AvatarUrl
            },
            InvitedBy = new UserDto
            {
                Id = invitedBy!.Id,
                Email = invitedBy.Email,
                DisplayName = invitedBy.DisplayName,
                AvatarUrl = invitedBy.AvatarUrl
            },
            Status = invitation.Status,
            CreatedAt = invitation.CreatedAt
        };
    }

    /// <summary>
    /// Get pending invitations for a user
    /// </summary>
    public async Task<List<InvitationDto>> GetPendingInvitationsAsync(Guid userId)
    {
        return await _db.Invitations
            .Include(i => i.Project)
            .Include(i => i.InvitedUser)
            .Include(i => i.InvitedByUser)
            .Where(i => i.InvitedUserId == userId && i.Status == InvitationStatus.Pending)
            .OrderByDescending(i => i.CreatedAt)
            .Select(i => new InvitationDto
            {
                Id = i.Id,
                ProjectId = i.ProjectId,
                ProjectName = i.Project.Name,
                ProjectColor = i.Project.Color,
                InvitedUser = new UserDto
                {
                    Id = i.InvitedUser.Id,
                    Email = i.InvitedUser.Email,
                    DisplayName = i.InvitedUser.DisplayName,
                    AvatarUrl = i.InvitedUser.AvatarUrl
                },
                InvitedBy = new UserDto
                {
                    Id = i.InvitedByUser.Id,
                    Email = i.InvitedByUser.Email,
                    DisplayName = i.InvitedByUser.DisplayName,
                    AvatarUrl = i.InvitedByUser.AvatarUrl
                },
                Status = i.Status,
                CreatedAt = i.CreatedAt
            })
            .ToListAsync();
    }

    /// <summary>
    /// Accept an invitation
    /// </summary>
    public async Task<bool> AcceptInvitationAsync(Guid invitationId, Guid userId)
    {
        var invitation = await _db.Invitations
            .Include(i => i.Project)
            .FirstOrDefaultAsync(i => i.Id == invitationId && i.InvitedUserId == userId);

        if (invitation == null || invitation.Status != InvitationStatus.Pending)
            return false;

        // Create project membership
        var membership = new ProjectMember
        {
            Id = Guid.NewGuid(),
            ProjectId = invitation.ProjectId,
            UserId = userId,
            Role = ProjectRole.Member,
            JoinedAt = DateTime.UtcNow
        };

        _db.ProjectMembers.Add(membership);

        // Update invitation status
        invitation.Status = InvitationStatus.Accepted;
        invitation.RespondedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Decline an invitation
    /// </summary>
    public async Task<bool> DeclineInvitationAsync(Guid invitationId, Guid userId)
    {
        var invitation = await _db.Invitations
            .FirstOrDefaultAsync(i => i.Id == invitationId && i.InvitedUserId == userId);

        if (invitation == null || invitation.Status != InvitationStatus.Pending)
            return false;

        invitation.Status = InvitationStatus.Declined;
        invitation.RespondedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Get project members
    /// </summary>
    public async Task<List<ProjectMemberDto>> GetProjectMembersAsync(Guid projectId, Guid userId)
    {
        // Check if user has access to project
        var project = await _db.Projects
            .Include(p => p.Members)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        if (project == null) return new List<ProjectMemberDto>();

        bool isOwner = project.OwnerId == userId;
        bool isMember = project.Members.Any(m => m.UserId == userId);
        if (!isOwner && !isMember) return new List<ProjectMemberDto>();

        var members = await _db.ProjectMembers
            .Include(pm => pm.User)
            .Where(pm => pm.ProjectId == projectId)
            .Select(pm => new ProjectMemberDto
            {
                Id = pm.Id,
                User = new UserDto
                {
                    Id = pm.User.Id,
                    Email = pm.User.Email,
                    DisplayName = pm.User.DisplayName,
                    AvatarUrl = pm.User.AvatarUrl
                },
                Role = pm.Role,
                JoinedAt = pm.JoinedAt
            })
            .ToListAsync();

        // Add owner as first member
        var owner = await _db.Users.FindAsync(project.OwnerId);
        if (owner != null)
        {
            members.Insert(0, new ProjectMemberDto
            {
                Id = Guid.Empty,
                User = new UserDto
                {
                    Id = owner.Id,
                    Email = owner.Email,
                    DisplayName = owner.DisplayName,
                    AvatarUrl = owner.AvatarUrl
                },
                Role = ProjectRole.Admin, // Owner is treated as Admin
                JoinedAt = project.CreatedAt,
                IsOwner = true
            });
        }

        return members;
    }
}
