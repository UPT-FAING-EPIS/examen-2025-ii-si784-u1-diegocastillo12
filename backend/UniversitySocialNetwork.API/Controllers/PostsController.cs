using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversitySocialNetwork.Core.DTOs;
using UniversitySocialNetwork.Core.Entities;
using UniversitySocialNetwork.Infrastructure.Data;

namespace UniversitySocialNetwork.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PostsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/posts/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PostDto>> GetPost(int id)
        {
            var post = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Group)
                .Include(p => p.Comments.Where(c => c.IsActive))
                    .ThenInclude(c => c.User)
                .Include(p => p.Reactions)
                    .ThenInclude(r => r.User)
                .Where(p => p.Id == id && p.IsActive)
                .Select(p => new PostDto
                {
                    Id = p.Id,
                    Content = p.Content,
                    ImageUrl = p.ImageUrl,
                    UserId = p.UserId,
                    UserName = p.User.FirstName + " " + p.User.LastName,
                    UserProfileImage = p.User.ProfileImageUrl,
                    GroupId = p.GroupId,
                    GroupName = p.Group != null ? p.Group.Name : null,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    CommentsCount = p.Comments.Count(c => c.IsActive),
                    ReactionsCount = p.Reactions.Count()
                })
                .FirstOrDefaultAsync();

            if (post == null)
            {
                return NotFound();
            }

            return post;
        }

        // GET: api/posts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetPosts(
            [FromQuery] int? userId = null,
            [FromQuery] int? groupId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = _context.Posts
                .Include(p => p.User)
                .Include(p => p.Group)
                .Where(p => p.IsActive);

            if (userId.HasValue)
            {
                query = query.Where(p => p.UserId == userId.Value);
            }

            if (groupId.HasValue)
            {
                query = query.Where(p => p.GroupId == groupId.Value);
            }

            var posts = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PostDto
                {
                    Id = p.Id,
                    Content = p.Content,
                    ImageUrl = p.ImageUrl,
                    UserId = p.UserId,
                    UserName = p.User.FirstName + " " + p.User.LastName,
                    UserProfileImage = p.User.ProfileImageUrl,
                    GroupId = p.GroupId,
                    GroupName = p.Group != null ? p.Group.Name : null,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    CommentsCount = p.Comments.Count(c => c.IsActive),
                    ReactionsCount = p.Reactions.Count()
                })
                .ToListAsync();

            return posts;
        }

        // POST: api/posts
        [HttpPost]
        public async Task<ActionResult<PostDto>> CreatePost(CreatePostDto createPostDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verify user exists
            if (!await _context.Users.AnyAsync(u => u.Id == createPostDto.UserId && u.IsActive))
            {
                return BadRequest("User not found");
            }

            // Verify group exists if specified
            if (createPostDto.GroupId.HasValue && 
                !await _context.Groups.AnyAsync(g => g.Id == createPostDto.GroupId && g.IsActive))
            {
                return BadRequest("Group not found");
            }

            var post = new Post
            {
                Content = createPostDto.Content,
                ImageUrl = createPostDto.ImageUrl,
                UserId = createPostDto.UserId,
                GroupId = createPostDto.GroupId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            // Load the created post with related data
            var createdPost = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Group)
                .Where(p => p.Id == post.Id)
                .Select(p => new PostDto
                {
                    Id = p.Id,
                    Content = p.Content,
                    ImageUrl = p.ImageUrl,
                    UserId = p.UserId,
                    UserName = p.User.FirstName + " " + p.User.LastName,
                    UserProfileImage = p.User.ProfileImageUrl,
                    GroupId = p.GroupId,
                    GroupName = p.Group != null ? p.Group.Name : null,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    CommentsCount = 0,
                    ReactionsCount = 0
                })
                .FirstAsync();

            return CreatedAtAction(nameof(GetPost), new { id = post.Id }, createdPost);
        }

        // PUT: api/posts/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(int id, UpdatePostDto updatePostDto)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null || !post.IsActive)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(updatePostDto.Content))
                post.Content = updatePostDto.Content;
            
            if (updatePostDto.ImageUrl != null)
                post.ImageUrl = updatePostDto.ImageUrl;

            post.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await PostExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/posts/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            post.IsActive = false;
            post.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/posts/{id}/comments
        [HttpGet("{id}/comments")]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetPostComments(int id)
        {
            var comments = await _context.Comments
                .Include(c => c.User)
                .Where(c => c.PostId == id && c.IsActive)
                .OrderBy(c => c.CreatedAt)
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
                .ToListAsync();

            return comments;
        }

        // GET: api/posts/{id}/reactions
        [HttpGet("{id}/reactions")]
        public async Task<ActionResult<IEnumerable<PostReactionDto>>> GetPostReactions(int id)
        {
            var reactions = await _context.PostReactions
                .Include(r => r.User)
                .Where(r => r.PostId == id)
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

        private async Task<bool> PostExists(int id)
        {
            return await _context.Posts.AnyAsync(e => e.Id == id && e.IsActive);
        }
    }
}