using System.ComponentModel.DataAnnotations;

namespace UniversitySocialNetwork.Core.DTOs
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public int PostId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? UserProfileImage { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateCommentDto
    {
        [Required]
        [StringLength(1000)]
        public string Content { get; set; } = string.Empty;
        
        [Required]
        public int PostId { get; set; }
        
        [Required]
        public int UserId { get; set; }
    }

    public class UpdateCommentDto
    {
        [Required]
        public string Content { get; set; } = string.Empty;
    }
}