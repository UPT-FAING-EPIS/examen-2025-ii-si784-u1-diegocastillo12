using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversitySocialNetwork.Core.DTOs;
using UniversitySocialNetwork.Core.Entities;
using UniversitySocialNetwork.Infrastructure.Data;

namespace UniversitySocialNetwork.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CommentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/comments/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CommentDto>> GetComment(int id)
        {
            var comment = await _context.Comments
                .Include(c => c.User)
                .Where(c => c.Id == id && c.IsActive)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    PostId = c.PostId,
                    UserId = c.UserId,
                    UserName = c.User.FirstName + " " + c.User.LastName,
                    UserProfileImage = c.User.ProfileImageUrl,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (comment == null)
            {
                return NotFound();
            }

            return comment;
        }

        // POST: api/comments
        [HttpPost]
        public async Task<ActionResult<CommentDto>> CreateComment(CreateCommentDto createCommentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verify post exists
            if (!await _context.Posts.AnyAsync(p => p.Id == createCommentDto.PostId && p.IsActive))
            {
                return BadRequest("Post not found");
            }

            // Verify user exists
            if (!await _context.Users.AnyAsync(u => u.Id == createCommentDto.UserId && u.IsActive))
            {
                return BadRequest("User not found");
            }

            var comment = new Comment
            {
                Content = createCommentDto.Content,
                PostId = createCommentDto.PostId,
                UserId = createCommentDto.UserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            // Load the created comment with related data
            var createdComment = await _context.Comments
                .Include(c => c.User)
                .Where(c => c.Id == comment.Id)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    PostId = c.PostId,
                    UserId = c.UserId,
                    UserName = c.User.FirstName + " " + c.User.LastName,
                    UserProfileImage = c.User.ProfileImageUrl,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .FirstAsync();

            return CreatedAtAction(nameof(GetComment), new { id = comment.Id }, createdComment);
        }

        // PUT: api/comments/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(int id, UpdateCommentDto updateCommentDto)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null || !comment.IsActive)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(updateCommentDto.Content))
                comment.Content = updateCommentDto.Content;

            comment.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CommentExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/comments/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            comment.IsActive = false;
            comment.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> CommentExists(int id)
        {
            return await _context.Comments.AnyAsync(e => e.Id == id && e.IsActive);
        }
    }
}