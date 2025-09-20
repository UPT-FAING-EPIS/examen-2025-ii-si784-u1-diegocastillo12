using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversitySocialNetwork.Core.DTOs;
using UniversitySocialNetwork.Core.Entities;
using UniversitySocialNetwork.Infrastructure.Data;

namespace UniversitySocialNetwork.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GroupsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/groups/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<GroupDto>> GetGroup(int id)
        {
            var group = await _context.Groups
                .Include(g => g.CreatedByUser)
                .Include(g => g.Members)
                    .ThenInclude(m => m.User)
                .Where(g => g.Id == id && g.IsActive)
                .Select(g => new GroupDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    Description = g.Description,
                    ImageUrl = g.ImageUrl,
                    CreatedByUserId = g.CreatedByUserId,
                    CreatedByUserName = g.CreatedByUser.FirstName + " " + g.CreatedByUser.LastName,
                    Type = g.Type,
                    CreatedAt = g.CreatedAt,
                    UpdatedAt = g.UpdatedAt,
                    MembersCount = g.Members.Count(m => m.IsActive)
                })
                .FirstOrDefaultAsync();

            if (group == null)
            {
                return NotFound();
            }

            return group;
        }

        // GET: api/groups
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GroupDto>>> GetGroups(
            [FromQuery] string? search = null,
            [FromQuery] GroupType? type = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = _context.Groups
                .Include(g => g.CreatedByUser)
                .Where(g => g.IsActive);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(g => g.Name.Contains(search) || g.Description.Contains(search));
            }

            if (type.HasValue)
            {
                query = query.Where(g => g.Type == type.Value);
            }

            var groups = await query
                .OrderByDescending(g => g.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(g => new GroupDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    Description = g.Description,
                    ImageUrl = g.ImageUrl,
                    CreatedByUserId = g.CreatedByUserId,
                    CreatedByUserName = g.CreatedByUser.FirstName + " " + g.CreatedByUser.LastName,
                    Type = g.Type,
                    CreatedAt = g.CreatedAt,
                    UpdatedAt = g.UpdatedAt,
                    MembersCount = g.Members.Count(m => m.IsActive)
                })
                .ToListAsync();

            return groups;
        }

        // POST: api/groups
        [HttpPost]
        public async Task<ActionResult<GroupDto>> CreateGroup(CreateGroupDto createGroupDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verify user exists
            if (!await _context.Users.AnyAsync(u => u.Id == createGroupDto.CreatedByUserId && u.IsActive))
            {
                return BadRequest("User not found");
            }

            var group = new Group
            {
                Name = createGroupDto.Name,
                Description = createGroupDto.Description,
                ImageUrl = createGroupDto.ImageUrl,
                CreatedByUserId = createGroupDto.CreatedByUserId,
                Type = createGroupDto.Type,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Groups.Add(group);
            await _context.SaveChangesAsync();

            // Add creator as admin member
            var groupMember = new GroupMember
            {
                GroupId = group.Id,
                UserId = createGroupDto.CreatedByUserId,
                Role = GroupMemberRole.Admin,
                JoinedAt = DateTime.UtcNow
            };

            _context.GroupMembers.Add(groupMember);
            await _context.SaveChangesAsync();

            // Load the created group with related data
            var createdGroup = await _context.Groups
                .Include(g => g.CreatedByUser)
                .Where(g => g.Id == group.Id)
                .Select(g => new GroupDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    Description = g.Description,
                    ImageUrl = g.ImageUrl,
                    CreatedByUserId = g.CreatedByUserId,
                    CreatedByUserName = g.CreatedByUser.FirstName + " " + g.CreatedByUser.LastName,
                    Type = g.Type,
                    CreatedAt = g.CreatedAt,
                    UpdatedAt = g.UpdatedAt,
                    MembersCount = 1
                })
                .FirstAsync();

            return CreatedAtAction(nameof(GetGroup), new { id = group.Id }, createdGroup);
        }

        // PUT: api/groups/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGroup(int id, UpdateGroupDto updateGroupDto)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group == null || !group.IsActive)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(updateGroupDto.Name))
                group.Name = updateGroupDto.Name;
            
            if (updateGroupDto.Description != null)
                group.Description = updateGroupDto.Description;
            
            if (updateGroupDto.ImageUrl != null)
                group.ImageUrl = updateGroupDto.ImageUrl;
            
            if (updateGroupDto.Type.HasValue)
                group.Type = updateGroupDto.Type.Value;

            group.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await GroupExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/groups/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group == null)
            {
                return NotFound();
            }

            group.IsActive = false;
            group.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/groups/{id}/members
        [HttpGet("{id}/members")]
        public async Task<ActionResult<IEnumerable<GroupMemberDto>>> GetGroupMembers(int id)
        {
            var members = await _context.GroupMembers
                .Include(m => m.User)
                .Where(m => m.GroupId == id && m.IsActive)
                .Select(m => new GroupMemberDto
                {
                    Id = m.Id,
                    GroupId = m.GroupId,
                    UserId = m.UserId,
                    UserName = m.User.FirstName + " " + m.User.LastName,
                    UserProfileImage = m.User.ProfileImageUrl,
                    Role = m.Role,
                    JoinedAt = m.JoinedAt
                })
                .ToListAsync();

            return members;
        }

        // POST: api/groups/{id}/join
        [HttpPost("{id}/join")]
        public async Task<IActionResult> JoinGroup(int id, [FromBody] int userId)
        {
            // Verify group exists
            if (!await _context.Groups.AnyAsync(g => g.Id == id && g.IsActive))
            {
                return NotFound("Group not found");
            }

            // Verify user exists
            if (!await _context.Users.AnyAsync(u => u.Id == userId && u.IsActive))
            {
                return BadRequest("User not found");
            }

            // Check if user is already a member
            if (await _context.GroupMembers.AnyAsync(m => m.GroupId == id && m.UserId == userId && m.IsActive))
            {
                return BadRequest("User is already a member of this group");
            }

            var groupMember = new GroupMember
            {
                GroupId = id,
                UserId = userId,
                Role = GroupMemberRole.Member,
                JoinedAt = DateTime.UtcNow
            };

            _context.GroupMembers.Add(groupMember);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/groups/{id}/leave
        [HttpDelete("{id}/leave")]
        public async Task<IActionResult> LeaveGroup(int id, [FromBody] int userId)
        {
            var groupMember = await _context.GroupMembers
                .FirstOrDefaultAsync(m => m.GroupId == id && m.UserId == userId && m.IsActive);

            if (groupMember == null)
            {
                return NotFound("User is not a member of this group");
            }

            groupMember.IsActive = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> GroupExists(int id)
        {
            return await _context.Groups.AnyAsync(e => e.Id == id && e.IsActive);
        }
    }
}