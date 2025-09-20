using System.ComponentModel.DataAnnotations;
using UniversitySocialNetwork.Core.Entities;

namespace UniversitySocialNetwork.Core.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? StudentId { get; set; }
        public string? Department { get; set; }
        public string? AcademicYear { get; set; }
        public UserRole Role { get; set; }
        public string? Bio { get; set; }
        public string? ProfileImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateUserDto
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
        
        public string? StudentId { get; set; }
        public string? Department { get; set; }
        public string? AcademicYear { get; set; }
        public UserRole Role { get; set; } = UserRole.Student;
        public string? Bio { get; set; }
    }

    public class UpdateUserDto
    {
        [StringLength(100)]
        public string? FirstName { get; set; }
        
        [StringLength(100)]
        public string? LastName { get; set; }
        
        public string? StudentId { get; set; }
        public string? Department { get; set; }
        public string? AcademicYear { get; set; }
        public string? Bio { get; set; }
        public string? ProfileImageUrl { get; set; }
    }
}