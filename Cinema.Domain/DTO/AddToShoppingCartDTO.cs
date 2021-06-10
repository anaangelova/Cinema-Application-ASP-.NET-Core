using Cinema.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cinema.Domain.DTO
{
    public class AddToShoppingCartDTO
    {
        public Ticket SelectedTicket { get; set; }
        public Guid TicketId { get; set; }
        [Display(Name = "Quantity")]
        public int TicketQuantity { get; set; }
    }
}
