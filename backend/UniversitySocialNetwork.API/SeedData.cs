using Microsoft.EntityFrameworkCore;
using UniversitySocialNetwork.Core.Entities;
using UniversitySocialNetwork.Infrastructure.Data;
using BCrypt.Net;

namespace UniversitySocialNetwork.API
{
    public static class SeedData
    {
        public static async Task SeedUsersAsync(ApplicationDbContext context)
        {
            // Verificar si ya existen usuarios
            if (await context.Users.AnyAsync())
            {
                return; // Ya hay usuarios, no hacer nada
            }

            var users = new List<User>
            {
                new User
                {
                    Email = "admin@universidad.com",
                    FirstName = "Admin",
                    LastName = "Sistema",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                    Role = UserRole.Admin,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new User
                {
                    Email = "juan.perez@estudiante.com",
                    FirstName = "Juan",
                    LastName = "Pérez",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Estudiante123!"),
                    Role = UserRole.Student,
                    StudentId = "2021001",
                    Department = "Ingeniería de Sistemas",
                    AcademicYear = "4to año",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new User
                {
                    Email = "maria.garcia@estudiante.com",
                    FirstName = "María",
                    LastName = "García",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Estudiante123!"),
                    Role = UserRole.Student,
                    StudentId = "2021002",
                    Department = "Psicología",
                    AcademicYear = "3er año",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new User
                {
                    Email = "carlos.rodriguez@profesor.com",
                    FirstName = "Carlos",
                    LastName = "Rodríguez",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Profesor123!"),
                    Role = UserRole.Teacher,
                    Department = "Ingeniería de Sistemas",
                    Bio = "Profesor de Programación y Bases de Datos",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new User
                {
                    Email = "ana.martinez@estudiante.com",
                    FirstName = "Ana",
                    LastName = "Martínez",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Estudiante123!"),
                    Role = UserRole.Student,
                    StudentId = "2022001",
                    Department = "Medicina",
                    AcademicYear = "2do año",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new User
                {
                    Email = "test@test.com",
                    FirstName = "Usuario",
                    LastName = "Prueba",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test123!"),
                    Role = UserRole.Student,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                }
            };

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
        }

        public static async Task SeedPostsAsync(ApplicationDbContext context)
        {
            // Verificar si ya existen posts
            if (await context.Posts.AnyAsync())
            {
                return; // Ya hay posts, no hacer nada
            }

            // Obtener usuarios para asignar a los posts
            var users = await context.Users.ToListAsync();
            if (!users.Any()) return;

            var posts = new List<Post>
            {
                new Post
                {
                    Content = "¡Hola a todos! Acabo de terminar mi proyecto de programación. ¿Alguien más está emocionado por las vacaciones de verano? 🎉",
                    UserId = users.First(u => u.FirstName == "Juan").Id,
                    CreatedAt = DateTime.UtcNow.AddHours(-2),
                    UpdatedAt = DateTime.UtcNow.AddHours(-2),
                    IsActive = true
                },
                new Post
                {
                    Content = "Estudiando para los exámenes finales. ¿Alguien quiere formar un grupo de estudio para Matemáticas Discretas?",
                    UserId = users.First(u => u.FirstName == "María").Id,
                    CreatedAt = DateTime.UtcNow.AddHours(-5),
                    UpdatedAt = DateTime.UtcNow.AddHours(-5),
                    IsActive = true
                },
                new Post
                {
                    Content = "La conferencia de tecnología de hoy estuvo increíble. Aprendí mucho sobre inteligencia artificial y machine learning. ¡Muy recomendada para todos los estudiantes de ingeniería!",
                    UserId = users.First(u => u.FirstName == "Ana").Id,
                    CreatedAt = DateTime.UtcNow.AddHours(-8),
                    UpdatedAt = DateTime.UtcNow.AddHours(-8),
                    IsActive = true
                },
                new Post
                {
                    Content = "Recordatorio: La entrega del proyecto final de Bases de Datos es el próximo viernes. Asegúrense de incluir la documentación completa y los diagramas ER.",
                    UserId = users.First(u => u.FirstName == "Carlos").Id,
                    CreatedAt = DateTime.UtcNow.AddHours(-12),
                    UpdatedAt = DateTime.UtcNow.AddHours(-12),
                    IsActive = true
                },
                new Post
                {
                    Content = "¿Alguien sabe si la biblioteca estará abierta este fin de semana? Necesito acceso a algunos libros de referencia para mi tesis.",
                    UserId = users.First(u => u.Email == "test@test.com").Id,
                    CreatedAt = DateTime.UtcNow.AddHours(-24),
                    UpdatedAt = DateTime.UtcNow.AddHours(-24),
                    IsActive = true
                }
            };

            await context.Posts.AddRangeAsync(posts);
            await context.SaveChangesAsync();
        }
    }
}