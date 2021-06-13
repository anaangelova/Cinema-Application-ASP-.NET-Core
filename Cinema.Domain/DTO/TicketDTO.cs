using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cinema.Domain.DTO
{
    public class TicketDTO
    {
        
        public List<string> Genres { get; set; }
        public List<bool> areChecked { get; set; }
    }
}
