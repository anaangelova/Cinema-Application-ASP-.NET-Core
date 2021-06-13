using Cinema.Domain.Identity;
using Cinema.Repository.Interface;
using Cinema.Services.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cinema.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public IEnumerable<CinemaApplicationUser> getAllUsers()
        {
            return _userRepository.GetAll();
        }

        public CinemaApplicationUser getUser(string id)
        {
            return _userRepository.Get(id);
        }

        public bool isAdmin(string id)
        {
            CinemaApplicationUser user = _userRepository.Get(id);
            if (user.isAdmin)
                return true;
            return false;
        }
    }
}
