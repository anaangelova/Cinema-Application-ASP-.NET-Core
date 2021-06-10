using Cinema.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cinema.Domain.DomainModels
{
    public class ShoppingCart : BaseEntity
    {

        public virtual ICollection<TicketInShoppingCart> TicketsInShoppingCart { get; set; }

        public virtual CinemaApplicationUser Owner { get; set; } //one-one relation with owner
        public string OwnerId { get; set; }

    }
}
