using Cinema.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cinema.Domain.DomainModels
{
    public class Order : BaseEntity
    {
       
        public string UserId { get; set; }
        public CinemaApplicationUser User { get; set; } //za one-to-many relacijata so korisnik

        //many-many relacijata
        public virtual ICollection<TicketInOrder> Tickets { get; set; }
    }
}
