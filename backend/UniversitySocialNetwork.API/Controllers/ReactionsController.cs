using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversitySocialNetwork.Core.DTOs;
using UniversitySocialNetwork.Core.Entities;
using UniversitySocialNetwork.Infrastructure.Data;

namespace UniversitySocialNetwork.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReactionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReactionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/reactions
        [HttpPost]
        public async Task<ActionResult<PostReactionDto>> CreateReaction(CreatePostReactionDto createReactionDto, [FromQuery] int userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verify post exists
            if (!await _context.Posts.AnyAsync(p => p.Id == createReactionDto.PostId && p.IsActive))
            {
                return BadRequest("Post not found");
            }

            // Verify user exists
            if (!await _context.Users.AnyAsync(u => u.Id == userId && u.IsActive))
            {
                return BadRequest("User not found");
            }

            // Check if user already reacted to this post
            var existingReaction = await _context.PostReactions
                .FirstOrDefaultAsync(r => r.PostId == createReactionDto.PostId && r.UserId == userId);

            if (existingReaction != null)
            {
                // Update existing reaction
                existingReaction.Type = createReactionDto.Type;
                existingReaction.CreatedAt = DateTime.UtcNow;
            }
            else
            {
                // Create new reaction
                existingReaction = new PostReaction
                {
                    PostId = createReactionDto.PostId,
                    UserId = userId,
                    Type = createReactionDto.Type,
                    CreatedAt = DateTime.UtcNow
                };
                _context.PostReactions.Add(existingReaction);
            }

            await _context.SaveChangesAsync();

            // Load the reaction with related data
            var reactionDto = await _context.PostReactions
                .Include(r => r.User)
                .Where(r => r.Id == existingReaction.Id)
                .Select(r => new PostReactionDto
                {
                    Id = r.Id,
                    PostId = r.PostId,
                    UserId = r.UserId,
                    UserName = r.User.FirstName + " " + r.User.LastName,
                    Type = r.Type,
                    CreatedAt = r.CreatedAt
                })
                .FirstAsync();

            return Ok(reactionDto);
        }

        // DELETE: api/reactions/{postId}/{userId}
        [HttpDelete("{postId}/{userId}")]
        public async Task<IActionResult> DeleteReaction(int postId, int userId)
        {
            var reaction = await _context.PostReactions
                .FirstOrDefaultAsync(r => r.PostId == postId && r.UserId == userId);

            if (reaction == null)
            {
                return NotFound();
            }

            _context.PostReactions.Remove(reaction);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/reactions/post/{postId}
        [HttpGet("post/{postId}")]
        public async Task<ActionResult<IEnumerable<PostReactionDto>>> GetPostReactions(int postId)
        {
            var reactions = await _context.PostReactions
                .Include(r => r.User)
                .Where(r => r.PostId == postId)
                .Select(r => new PostReactionDto
                {
                    Id = r.Id,
                    PostId = r.PostId,
                    UserId = r.UserId,
                    UserName = r.User.FirstName + " " + r.User.LastName,
                    Type = r.Type,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();

            return reactions;
        }

        // GET: api/reactions/post/{postId}/summary
        [HttpGet("post/{postId}/summary")]
        public async Task<ActionResult<object>> GetPostReactionsSummary(int postId)
        {
            var reactionsSummary = await _context.PostReactions
                .Where(r => r.PostId == postId)
                .GroupBy(r => r.Type)
                .Select(g => new { Type = g.Key, Count = g.Count() })
                .ToListAsync();

            var totalReactions = reactionsSummary.Sum(r => r.Count);

            return Ok(new
            {
                PostId = postId,
                TotalReactions = totalReactions,
                ReactionsByType = reactionsSummary
            });
        }

        // GET: api/reactions/user/{userId}/post/{postId}
        [HttpGet("user/{userId}/post/{postId}")]
        public async Task<ActionResult<PostReactionDto?>> GetUserReactionForPost(int userId, int postId)
        {
            var reaction = await _context.PostReactions
                .Include(r => r.User)
                .Where(r => r.PostId == postId && r.UserId == userId)
                .Select(r => new PostReactionDto
                {
                    Id = r.Id,
                    PostId = r.PostId,
                    UserId = r.UserId,
                    UserName = r.User.FirstName + " " + r.User.LastName,
                    Type = r.Type,
                    CreatedAt = r.CreatedAt
                })
                .FirstOrDefaultAsync();

            return Ok(reaction);
        }
    }
}