using System.ComponentModel.DataAnnotations;

namespace UniversitySocialNetwork.Core.Entities
{
    public class Group
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string? Description { get; set; }
        
        [StringLength(255)]
        public string? ImageUrl { get; set; }
        
        public int CreatedByUserId { get; set; }
        
        public GroupType Type { get; set; } = GroupType.Public;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public virtual User CreatedByUser { get; set; } = null!;
        public virtual ICollection<GroupMember> Members { get; set; } = new List<GroupMember>();
        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    }
    
    public enum GroupType
    {
        Public = 1,
        Private = 2,
        Academic = 3
    }
}