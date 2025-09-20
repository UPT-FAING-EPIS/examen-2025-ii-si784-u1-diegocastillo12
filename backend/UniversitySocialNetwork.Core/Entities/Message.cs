using System.ComponentModel.DataAnnotations;

namespace UniversitySocialNetwork.Core.Entities
{
    public class Message
    {
        public int Id { get; set; }
        
        [Required]
        public string Content { get; set; } = string.Empty;
        
        public int SenderId { get; set; }
        
        public int ReceiverId { get; set; }
        
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? ReadAt { get; set; }
        
        public bool IsRead { get; set; } = false;
        
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public virtual User Sender { get; set; } = null!;
        public virtual User Receiver { get; set; } = null!;
    }
}