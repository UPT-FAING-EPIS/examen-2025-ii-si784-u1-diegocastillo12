using System.ComponentModel.DataAnnotations;

namespace UniversitySocialNetwork.Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string? StudentId { get; set; }
        
        [StringLength(100)]
        public string? Department { get; set; }
        
        [StringLength(50)]
        public string? AcademicYear { get; set; }
        
        public UserRole Role { get; set; } = UserRole.Student;
        
        [StringLength(500)]
        public string? Bio { get; set; }
        
        [StringLength(255)]
        public string? ProfileImageUrl { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<GroupMember> GroupMemberships { get; set; } = new List<GroupMember>();
        public virtual ICollection<Message> SentMessages { get; set; } = new List<Message>();
        public virtual ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();
        public virtual ICollection<PostReaction> PostReactions { get; set; } = new List<PostReaction>();
    }
    
    public enum UserRole
    {
        Student = 1,
        Teacher = 2,
        Staff = 3,
        Admin = 4
    }
}