using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cinema.Domain.DomainModels
{
    public class TicketInShoppingCart : BaseEntity
    {
        public Guid TicketId { get; set; }
        public Guid ShoppingCartId { get; set; }

        public Ticket Ticket { get; set; }
        public ShoppingCart ShoppingCart { get; set; }

        [Display(Name = "Quantity")]
        public int TicketQuantity { get; set; }
    }
}
