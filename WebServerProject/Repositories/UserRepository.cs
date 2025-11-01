using Microsoft.EntityFrameworkCore;
using WebServerProject.Data;
using WebServerProject.Models.Entities;

namespace WebServerProject.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(int userId);
        Task<User> GetByUsernameAsync(string username);
        Task<User> GetByEmailAsync(string email);
        Task<int> CreateAsync(User user);
        Task<bool> UpdateAsync(User user);
        Task<bool> UpdateLastLoginAsync(int userId, DateTime loginTime);
    }
    public class UserRepository : IUserRepository
    {
        private readonly AuthDbContext _db;

        public UserRepository(AuthDbContext db)
        {
            _db = db;
        }

        public Task<User> GetByIdAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetByUsernameAsync(string username)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<int> CreateAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateLastLoginAsync(int userId, DateTime loginTime)
        {
            throw new NotImplementedException();
        }
    }
}
