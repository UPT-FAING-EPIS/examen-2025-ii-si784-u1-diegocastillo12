using System.ComponentModel.DataAnnotations;
using UniversitySocialNetwork.Core.Entities;

namespace UniversitySocialNetwork.Core.DTOs
{
    public class MessageDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public int SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public int ReceiverId { get; set; }
        public string ReceiverName { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public bool IsRead { get; set; }
    }

    public class CreateMessageDto
    {
        [Required]
        public string Content { get; set; } = string.Empty;
        
        [Required]
        public int ReceiverId { get; set; }
    }

    public class GroupMemberDto
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? UserProfileImage { get; set; }
        public GroupMemberRole Role { get; set; }
        public DateTime JoinedAt { get; set; }
    }

    public class PostReactionDto
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public ReactionType Type { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreatePostReactionDto
    {
        [Required]
        public int PostId { get; set; }
        
        [Required]
        public ReactionType Type { get; set; }
    }
}