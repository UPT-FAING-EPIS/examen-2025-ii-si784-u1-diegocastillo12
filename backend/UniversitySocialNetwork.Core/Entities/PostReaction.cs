namespace UniversitySocialNetwork.Core.Entities
{
    public class PostReaction
    {
        public int Id { get; set; }
        
        public int PostId { get; set; }
        
        public int UserId { get; set; }
        
        public ReactionType Type { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual Post Post { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
    
    public enum ReactionType
    {
        Like = 1,
        Love = 2,
        Laugh = 3,
        Angry = 4,
        Sad = 5
    }
}