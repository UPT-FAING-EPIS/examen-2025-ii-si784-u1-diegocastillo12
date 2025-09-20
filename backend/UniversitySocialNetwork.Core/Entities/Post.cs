using System.ComponentModel.DataAnnotations;

namespace UniversitySocialNetwork.Core.Entities
{
    public class Post
    {
        public int Id { get; set; }
        
        [Required]
        public string Content { get; set; } = string.Empty;
        
        [StringLength(255)]
        public string? ImageUrl { get; set; }
        
        public int UserId { get; set; }
        
        public int? GroupId { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual Group? Group { get; set; }
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<PostReaction> Reactions { get; set; } = new List<PostReaction>();
    }
}