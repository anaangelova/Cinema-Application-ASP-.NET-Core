using Cinema.Domain.DomainModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;


namespace Cinema.Domain.Identity
{
    public class CinemaApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public bool isAdmin { get; set; }
        public virtual ShoppingCart UserCart { get; set; }
    }
}
