using System.ComponentModel.DataAnnotations;

namespace UniversitySocialNetwork.Core.DTOs
{
    public class PostDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? UserProfileImage { get; set; }
        public int? GroupId { get; set; }
        public string? GroupName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int CommentsCount { get; set; }
        public int ReactionsCount { get; set; }
        public List<CommentDto> Comments { get; set; } = new List<CommentDto>();
        public List<PostReactionDto> Reactions { get; set; } = new List<PostReactionDto>();
    }

    public class CreatePostDto
    {
        [Required]
        [StringLength(2000)]
        public string Content { get; set; } = string.Empty;
        
        public string? ImageUrl { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        public int? GroupId { get; set; }
    }

    public class UpdatePostDto
    {
        public string? Content { get; set; }
        public string? ImageUrl { get; set; }
    }
}