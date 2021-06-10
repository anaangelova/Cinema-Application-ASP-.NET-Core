using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cinema.Domain.DomainModels
{
    public class Ticket : BaseEntity
    {


        [Required]
        [Display(Name = "Ticket Title")]
        public string TicketName { get; set; }
        [Required]
        [Display(Name = "Description")]
        public string TicketDescription { get; set; }
        [Required]
        [Display(Name = "Genre")]
        public string TicketGenre { get; set; }
        [Required]
        [Display(Name = "Price")]
        public int TicketPrice { get; set; }
        [Required]
        [Display(Name = "Image")]
        public string TicketImage { get; set; }
        [Required]
        [Display(Name = "Date")]
        public DateTime TicketDate { get; set; }

        public virtual ICollection<TicketInShoppingCart> TicketsInShoppingCart { get; set; } //many many relation

        public virtual ICollection<TicketInOrder> Orders { get; set; }



    }
}
