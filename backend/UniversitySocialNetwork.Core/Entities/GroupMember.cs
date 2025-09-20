namespace UniversitySocialNetwork.Core.Entities
{
    public class GroupMember
    {
        public int Id { get; set; }
        
        public int GroupId { get; set; }
        
        public int UserId { get; set; }
        
        public GroupMemberRole Role { get; set; } = GroupMemberRole.Member;
        
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public virtual Group Group { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
    
    public enum GroupMemberRole
    {
        Member = 1,
        Moderator = 2,
        Admin = 3
    }
}