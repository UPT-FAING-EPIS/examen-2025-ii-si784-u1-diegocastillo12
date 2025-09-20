using UniversitySocialNetwork.Core.Entities;

namespace UniversitySocialNetwork.Core.Services
{
    public interface ISearchService
    {
        Task<IEnumerable<User>> SearchUsersAsync(string query, int page = 1, int pageSize = 10);
        Task<IEnumerable<Post>> SearchPostsAsync(string query, int page = 1, int pageSize = 10);
        Task<IEnumerable<User>> SearchUsersByUniversityAsync(string universityName, int page = 1, int pageSize = 10);
        Task<IEnumerable<User>> SearchUsersByCareerAsync(string careerName, int page = 1, int pageSize = 10);
        Task<SearchResultsDto> SearchAllAsync(string query, int page = 1, int pageSize = 10);
    }

    public class SearchResultsDto
    {
        public IEnumerable<User> Users { get; set; } = new List<User>();
        public IEnumerable<Post> Posts { get; set; } = new List<Post>();
        public int TotalUsers { get; set; }
        public int TotalPosts { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }
}