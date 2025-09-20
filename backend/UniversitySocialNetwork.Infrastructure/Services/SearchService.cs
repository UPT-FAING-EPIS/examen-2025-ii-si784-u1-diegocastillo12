using Microsoft.EntityFrameworkCore;
using UniversitySocialNetwork.Core.Entities;
using UniversitySocialNetwork.Core.Services;
using UniversitySocialNetwork.Infrastructure.Data;

namespace UniversitySocialNetwork.Infrastructure.Services
{
    public class SearchService : ISearchService
    {
        private readonly ApplicationDbContext _context;

        public SearchService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> SearchUsersAsync(string query, int page = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<User>();

            var normalizedQuery = query.ToLower().Trim();

            return await _context.Users
                .Where(u => (u.FirstName + " " + u.LastName).ToLower().Contains(normalizedQuery) ||
                           u.Email.ToLower().Contains(normalizedQuery) ||
                           (u.Department != null && u.Department.ToLower().Contains(normalizedQuery)) ||
                           (u.StudentId != null && u.StudentId.ToLower().Contains(normalizedQuery)))
                .OrderBy(u => u.FirstName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Post>> SearchPostsAsync(string query, int page = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<Post>();

            var normalizedQuery = query.ToLower().Trim();

            return await _context.Posts
                .Include(p => p.User)
                .Where(p => p.Content.ToLower().Contains(normalizedQuery))
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> SearchUsersByUniversityAsync(string universityName, int page = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(universityName))
                return new List<User>();

            var normalizedUniversity = universityName.ToLower().Trim();

            return await _context.Users
                .Where(u => u.Department != null && u.Department.ToLower().Contains(normalizedUniversity))
                .OrderBy(u => u.FirstName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> SearchUsersByCareerAsync(string careerName, int page = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(careerName))
                return new List<User>();

            var normalizedCareer = careerName.ToLower().Trim();

            return await _context.Users
                .Where(u => u.Department != null && u.Department.ToLower().Contains(normalizedCareer))
                .OrderBy(u => u.FirstName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<SearchResultsDto> SearchAllAsync(string query, int page = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return new SearchResultsDto
                {
                    Users = new List<User>(),
                    Posts = new List<Post>(),
                    TotalUsers = 0,
                    TotalPosts = 0,
                    CurrentPage = page,
                    PageSize = pageSize
                };
            }

            var normalizedQuery = query.ToLower().Trim();

            // Buscar usuarios
            var usersQuery = _context.Users
                .Where(u => (u.FirstName + " " + u.LastName).ToLower().Contains(normalizedQuery) ||
                           u.Email.ToLower().Contains(normalizedQuery) ||
                           (u.Department != null && u.Department.ToLower().Contains(normalizedQuery)) ||
                           (u.StudentId != null && u.StudentId.ToLower().Contains(normalizedQuery)));

            var totalUsers = await usersQuery.CountAsync();
            var users = await usersQuery
                .OrderBy(u => u.FirstName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Buscar posts
            var postsQuery = _context.Posts
                .Include(p => p.User)
                .Where(p => p.Content.ToLower().Contains(normalizedQuery));

            var totalPosts = await postsQuery.CountAsync();
            var posts = await postsQuery
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new SearchResultsDto
            {
                Users = users,
                Posts = posts,
                TotalUsers = totalUsers,
                TotalPosts = totalPosts,
                CurrentPage = page,
                PageSize = pageSize
            };
        }
    }
}