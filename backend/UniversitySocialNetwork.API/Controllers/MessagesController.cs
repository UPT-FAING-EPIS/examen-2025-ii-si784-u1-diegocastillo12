using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversitySocialNetwork.Core.DTOs;
using UniversitySocialNetwork.Core.Entities;
using UniversitySocialNetwork.Infrastructure.Data;

namespace UniversitySocialNetwork.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MessagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/messages/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<MessageDto>> GetMessage(int id)
        {
            var message = await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m => m.Id == id && m.IsActive)
                .Select(m => new MessageDto
                {
                    Id = m.Id,
                    Content = m.Content,
                    SenderId = m.SenderId,
                    SenderName = m.Sender.FirstName + " " + m.Sender.LastName,
                    ReceiverId = m.ReceiverId,
                    ReceiverName = m.Receiver.FirstName + " " + m.Receiver.LastName,
                    SentAt = m.SentAt,
                    ReadAt = m.ReadAt,
                    IsRead = m.IsRead
                })
                .FirstOrDefaultAsync();

            if (message == null)
            {
                return NotFound();
            }

            return message;
        }

        // GET: api/messages/conversations/{userId}
        [HttpGet("conversations/{userId}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetUserConversations(int userId)
        {
            var conversations = await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m => (m.SenderId == userId || m.ReceiverId == userId) && m.IsActive)
                .GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                .Select(g => g.OrderByDescending(m => m.SentAt).First())
                .Select(m => new MessageDto
                {
                    Id = m.Id,
                    Content = m.Content,
                    SenderId = m.SenderId,
                    SenderName = m.Sender.FirstName + " " + m.Sender.LastName,
                    ReceiverId = m.ReceiverId,
                    ReceiverName = m.Receiver.FirstName + " " + m.Receiver.LastName,
                    SentAt = m.SentAt,
                    ReadAt = m.ReadAt,
                    IsRead = m.IsRead
                })
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();

            return conversations;
        }

        // GET: api/messages/conversation/{userId1}/{userId2}
        [HttpGet("conversation/{userId1}/{userId2}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetConversation(
            int userId1, 
            int userId2,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            var messages = await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m => ((m.SenderId == userId1 && m.ReceiverId == userId2) ||
                            (m.SenderId == userId2 && m.ReceiverId == userId1)) && m.IsActive)
                .OrderByDescending(m => m.SentAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new MessageDto
                {
                    Id = m.Id,
                    Content = m.Content,
                    SenderId = m.SenderId,
                    SenderName = m.Sender.FirstName + " " + m.Sender.LastName,
                    ReceiverId = m.ReceiverId,
                    ReceiverName = m.Receiver.FirstName + " " + m.Receiver.LastName,
                    SentAt = m.SentAt,
                    ReadAt = m.ReadAt,
                    IsRead = m.IsRead
                })
                .ToListAsync();

            return messages;
        }

        // POST: api/messages
        [HttpPost]
        public async Task<ActionResult<MessageDto>> SendMessage(CreateMessageDto createMessageDto, [FromQuery] int senderId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verify sender exists
            if (!await _context.Users.AnyAsync(u => u.Id == senderId && u.IsActive))
            {
                return BadRequest("Sender not found");
            }

            // Verify receiver exists
            if (!await _context.Users.AnyAsync(u => u.Id == createMessageDto.ReceiverId && u.IsActive))
            {
                return BadRequest("Receiver not found");
            }

            var message = new Message
            {
                Content = createMessageDto.Content,
                SenderId = senderId,
                ReceiverId = createMessageDto.ReceiverId,
                SentAt = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            // Load the created message with related data
            var createdMessage = await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m => m.Id == message.Id)
                .Select(m => new MessageDto
                {
                    Id = m.Id,
                    Content = m.Content,
                    SenderId = m.SenderId,
                    SenderName = m.Sender.FirstName + " " + m.Sender.LastName,
                    ReceiverId = m.ReceiverId,
                    ReceiverName = m.Receiver.FirstName + " " + m.Receiver.LastName,
                    SentAt = m.SentAt,
                    ReadAt = m.ReadAt,
                    IsRead = m.IsRead
                })
                .FirstAsync();

            return CreatedAtAction(nameof(GetMessage), new { id = message.Id }, createdMessage);
        }

        // PUT: api/messages/{id}/read
        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message == null || !message.IsActive)
            {
                return NotFound();
            }

            if (!message.IsRead)
            {
                message.IsRead = true;
                message.ReadAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            return NoContent();
        }

        // DELETE: api/messages/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            message.IsActive = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/messages/unread/{userId}
        [HttpGet("unread/{userId}")]
        public async Task<ActionResult<int>> GetUnreadCount(int userId)
        {
            var unreadCount = await _context.Messages
                .CountAsync(m => m.ReceiverId == userId && !m.IsRead && m.IsActive);

            return unreadCount;
        }
    }
}