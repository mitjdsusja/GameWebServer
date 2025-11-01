using System.Reflection;
using WebServerProject.Data;
using WebServerProject.Models.DTOs;
using WebServerProject.Repositories;

namespace WebServerProject.Services
{
    public class HomeService
    {
        private readonly UserRepository _userRepo;

        public HomeService(UserRepository playerRepo)
        {
            _userRepo = playerRepo;
        }
    }
}
