using Cinema.Domain.DomainModels;
using Cinema.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinema.Repository.Implementation
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext context;
        private DbSet<Order> entities;
        public OrderRepository(ApplicationDbContext _context)
        {
            context = _context;
            entities = context.Set<Order>();
        }
        public List<Order> getAllOrders(string userId)
        {
            var allOrders = entities
               .Where(z => z.UserId.Equals(userId))
               .Include(z => z.User)
               .Include(z => z.Tickets)
               .Include("Tickets.SelectedTicket")
               .ToList();
            return allOrders;

        }

      
        public Order getOrderDetails(Guid id)
        {
            return  entities.Include(z => z.User)
                .Include(z => z.Tickets)
                .Include("Tickets.SelectedTicket")
                .SingleOrDefaultAsync(z => z.Id.Equals(id)).Result;
        }
    }
}
