using Cinema.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cinema.Services.Interface
{
    public interface IUserService
    {
        public IEnumerable<CinemaApplicationUser> getAllUsers();
        public bool isAdmin(string id);
        public CinemaApplicationUser getUser(string id);
    }
}
