using Cinema.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cinema.Repository.Interface
{
    public interface ITicketRepository
    {
        IEnumerable<Ticket> GetFilteredTickets(DateTime? start, DateTime? end);
    }
}
