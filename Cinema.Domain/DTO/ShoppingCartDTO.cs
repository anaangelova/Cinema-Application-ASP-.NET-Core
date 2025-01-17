﻿using Cinema.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cinema.Domain.DTO
{
    public class ShoppingCartDTO
    {
        public List<TicketInShoppingCart> TicketInShoppingCart { get; set; }
        public double TotalPrice { get; set; }
    }
}
