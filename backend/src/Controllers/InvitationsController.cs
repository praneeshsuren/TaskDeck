using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskDeck.Api.Models;
using TaskDeck.Api.Services;

namespace TaskDeck.Api.Controllers;

/// <summary>
/// Controller for managing project invitations
/// </summary>
[Authorize]
[ApiController]
[Route("api/invitations")]
public class InvitationsController : ControllerBase
{
    private readonly InvitationService _invitationService;

    public InvitationsController(InvitationService invitationService)
    {
        _invitationService = invitationService;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim!);
    }

    /// <summary>
    /// Get pending invitations for the current user
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<InvitationDto>>> GetPendingInvitations()
    {
        var userId = GetUserId();
        var invitations = await _invitationService.GetPendingInvitationsAsync(userId);
        return Ok(invitations);
    }

    /// <summary>
    /// Accept an invitation
    /// </summary>
    [HttpPost("{id}/accept")]
    public async Task<IActionResult> AcceptInvitation(Guid id)
    {
        var userId = GetUserId();
        var success = await _invitationService.AcceptInvitationAsync(id, userId);
        
        if (!success)
            return NotFound(new { message = "Invitation not found or already responded" });

        return Ok(new { message = "Invitation accepted" });
    }

    /// <summary>
    /// Decline an invitation
    /// </summary>
    [HttpPost("{id}/decline")]
    public async Task<IActionResult> DeclineInvitation(Guid id)
    {
        var userId = GetUserId();
        var success = await _invitationService.DeclineInvitationAsync(id, userId);
        
        if (!success)
            return NotFound(new { message = "Invitation not found or already responded" });

        return Ok(new { message = "Invitation declined" });
    }
}

/// <summary>
/// Controller for project-specific invitation operations
/// </summary>
[Authorize]
[ApiController]
[Route("api/projects/{projectId}/invitations")]
public class ProjectInvitationsController : ControllerBase
{
    private readonly InvitationService _invitationService;

    public ProjectInvitationsController(InvitationService invitationService)
    {
        _invitationService = invitationService;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim!);
    }

    /// <summary>
    /// Send an invitation to join a project
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<InvitationDto>> SendInvitation(Guid projectId, [FromBody] SendInvitationDto dto)
    {
        try
        {
            var userId = GetUserId();
            var invitation = await _invitationService.SendInvitationAsync(projectId, dto.Email, userId);
            
            if (invitation == null)
                return NotFound(new { message = "Project not found" });

            return Ok(invitation);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

/// <summary>
/// Controller for project members
/// </summary>
[Authorize]
[ApiController]
[Route("api/projects/{projectId}/members")]
public class ProjectMembersController : ControllerBase
{
    private readonly InvitationService _invitationService;

    public ProjectMembersController(InvitationService invitationService)
    {
        _invitationService = invitationService;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim!);
    }

    /// <summary>
    /// Get project members
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<ProjectMemberDto>>> GetMembers(Guid projectId)
    {
        var userId = GetUserId();
        var members = await _invitationService.GetProjectMembersAsync(projectId, userId);
        return Ok(members);
    }
}
