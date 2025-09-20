using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversitySocialNetwork.Core.Services;

namespace UniversitySocialNetwork.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpGet("users")]
        public async Task<IActionResult> SearchUsers(
            [FromQuery] string query,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return BadRequest(new { message = "Query parameter is required" });
                }

                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 50) pageSize = 10;

                var users = await _searchService.SearchUsersAsync(query, page, pageSize);

                var result = users.Select(u => new
                {
                    id = u.Id,
                    name = u.FirstName + " " + u.LastName,
                    email = u.Email,
                    department = u.Department,
                    studentId = u.StudentId,
                    profilePictureUrl = u.ProfileImageUrl,
                    createdAt = u.CreatedAt
                });

                return Ok(new
                {
                    data = result,
                    page,
                    pageSize,
                    query
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error searching users", error = ex.Message });
            }
        }

        [HttpGet("posts")]
        public async Task<IActionResult> SearchPosts(
            [FromQuery] string query,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return BadRequest(new { message = "Query parameter is required" });
                }

                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 50) pageSize = 10;

                var posts = await _searchService.SearchPostsAsync(query, page, pageSize);

                var result = posts.Select(p => new
                {
                    id = p.Id,
                    content = p.Content,
                    imageUrl = p.ImageUrl,
                    createdAt = p.CreatedAt,
                    user = new
                    {
                        id = p.User.Id,
                        name = p.User.FirstName + " " + p.User.LastName,
                        profilePictureUrl = p.User.ProfileImageUrl
                    }
                });

                return Ok(new
                {
                    data = result,
                    page,
                    pageSize,
                    query
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error searching posts", error = ex.Message });
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> SearchAll(
            [FromQuery] string query,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return BadRequest(new { message = "Query parameter is required" });
                }

                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 50) pageSize = 10;

                var searchResults = await _searchService.SearchAllAsync(query, page, pageSize);

                var users = searchResults.Users.Select(u => new
                {
                    id = u.Id,
                    name = u.FirstName + " " + u.LastName,
                    email = u.Email,
                    department = u.Department,
                    studentId = u.StudentId,
                    profilePictureUrl = u.ProfileImageUrl,
                    createdAt = u.CreatedAt
                });

                var posts = searchResults.Posts.Select(p => new
                {
                    id = p.Id,
                    content = p.Content,
                    imageUrl = p.ImageUrl,
                    createdAt = p.CreatedAt,
                    user = new
                    {
                        id = p.User.Id,
                        name = p.User.FirstName + " " + p.User.LastName,
                        profilePictureUrl = p.User.ProfileImageUrl
                    }
                });

                return Ok(new
                {
                    users = new
                    {
                        data = users,
                        total = searchResults.TotalUsers
                    },
                    posts = new
                    {
                        data = posts,
                        total = searchResults.TotalPosts
                    },
                    page = searchResults.CurrentPage,
                    pageSize = searchResults.PageSize,
                    query
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error performing search", error = ex.Message });
            }
        }

        [HttpGet("users/university")]
        public async Task<IActionResult> SearchUsersByUniversity(
            [FromQuery] string university,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(university))
                {
                    return BadRequest(new { message = "University parameter is required" });
                }

                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 50) pageSize = 10;

                var users = await _searchService.SearchUsersByUniversityAsync(university, page, pageSize);

                var result = users.Select(u => new
                {
                    id = u.Id,
                    name = u.FirstName + " " + u.LastName,
                    email = u.Email,
                    department = u.Department,
                    studentId = u.StudentId,
                    profilePictureUrl = u.ProfileImageUrl,
                    createdAt = u.CreatedAt
                });

                return Ok(new
                {
                    data = result,
                    page,
                    pageSize,
                    university
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error searching users by university", error = ex.Message });
            }
        }

        [HttpGet("users/career")]
        public async Task<IActionResult> SearchUsersByCareer(
            [FromQuery] string career,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(career))
                {
                    return BadRequest(new { message = "Career parameter is required" });
                }

                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 50) pageSize = 10;

                var users = await _searchService.SearchUsersByCareerAsync(career, page, pageSize);

                var result = users.Select(u => new
                {
                    id = u.Id,
                    name = u.FirstName + " " + u.LastName,
                    email = u.Email,
                    department = u.Department,
                    studentId = u.StudentId,
                    profilePictureUrl = u.ProfileImageUrl,
                    createdAt = u.CreatedAt
                });

                return Ok(new
                {
                    data = result,
                    page,
                    pageSize,
                    career
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error searching users by career", error = ex.Message });
            }
        }
    }
}