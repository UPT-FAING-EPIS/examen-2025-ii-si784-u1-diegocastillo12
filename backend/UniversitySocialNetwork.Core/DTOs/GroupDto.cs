using System.ComponentModel.DataAnnotations;
using UniversitySocialNetwork.Core.Entities;

namespace UniversitySocialNetwork.Core.DTOs
{
    public class GroupDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int CreatedByUserId { get; set; }
        public string CreatedByUserName { get; set; } = string.Empty;
        public GroupType Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int MembersCount { get; set; }
        public int PostsCount { get; set; }
        public List<GroupMemberDto> Members { get; set; } = new List<GroupMemberDto>();
    }

    public class CreateGroupDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string? Description { get; set; }
        
        public string? ImageUrl { get; set; }
        
        [Required]
        public int CreatedByUserId { get; set; }
        
        [Required]
        public GroupType Type { get; set; } = GroupType.Public;
    }

    public class UpdateGroupDto
    {
        [StringLength(200)]
        public string? Name { get; set; }
        
        [StringLength(1000)]
        public string? Description { get; set; }
        
        public string? ImageUrl { get; set; }
        public GroupType? Type { get; set; }
    }
}