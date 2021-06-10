using Cinema.Domain.DomainModels;
using Cinema.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinema.Repository.Implementation
{
    public class TicketRepository : ITicketRepository
    {
        private readonly ApplicationDbContext context;
        private DbSet<Ticket> entities;
        public TicketRepository(ApplicationDbContext _context)
        {
            context = _context;
            entities = context.Set<Ticket>();
        }
        public IEnumerable<Ticket> GetFilteredTickets(DateTime? start, DateTime? end)
        {
            return entities.Where(z => z.TicketDate >= start && z.TicketDate <= end).ToList();
        }
    }
}
