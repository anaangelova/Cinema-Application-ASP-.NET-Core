using Cinema.Domain.Identity;
using Cinema.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinema.Repository.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext context;
        private DbSet<CinemaApplicationUser> entities;
     
        public UserRepository(ApplicationDbContext context)
        {
            this.context = context;
            entities = context.Set<CinemaApplicationUser>();
        }
        public void Delete(CinemaApplicationUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Remove(entity);
            context.SaveChanges();
        }

        public CinemaApplicationUser Get(string id)
        {
            return entities
               .Include(z => z.UserCart)
               .Include("UserCart.TicketsInShoppingCart")
               .Include("UserCart.TicketsInShoppingCart.Ticket")
               .SingleOrDefault(s => s.Id == id);
        }

        public IEnumerable<CinemaApplicationUser> GetAll()
        {
            return entities.AsEnumerable();
        }

        public void Insert(CinemaApplicationUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Add(entity);
            context.SaveChanges();
        }

        public void Update(CinemaApplicationUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Update(entity);
            context.SaveChanges();
        }
    }
}
