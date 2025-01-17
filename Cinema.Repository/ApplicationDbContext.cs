﻿using Cinema.Domain.DomainModels;
using Cinema.Domain.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cinema.Repository
{
    public class ApplicationDbContext : IdentityDbContext<CinemaApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Ticket> Tickets { get; set; }
        public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public virtual DbSet<TicketInShoppingCart> TicketInShoppingCarts { get; set; }
        public virtual DbSet<Order> Order { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

           builder.Entity<Ticket>().Property(z => z.Id).ValueGeneratedOnAdd();
            builder.Entity<ShoppingCart>().Property(z => z.Id).ValueGeneratedOnAdd();

            /*builder.Entity<TicketInShoppingCart>().HasKey(z => new { z.TicketId, z.ShoppingCartId });*/

            builder.Entity<TicketInShoppingCart>()
                .HasOne(z => z.Ticket)
                .WithMany(z => z.TicketsInShoppingCart)
                .HasForeignKey(z => z.ShoppingCartId);

            builder.Entity<TicketInShoppingCart>()
                .HasOne(z => z.ShoppingCart)
                .WithMany(z => z.TicketsInShoppingCart)
                .HasForeignKey(z => z.TicketId);

            builder.Entity<ShoppingCart>().HasOne(z => z.Owner).WithOne(z => z.UserCart).HasForeignKey<ShoppingCart>(z => z.OwnerId);

            builder.Entity<TicketInOrder>()
               .HasOne(z => z.SelectedTicket) 
               .WithMany(z => z.Orders)
               .HasForeignKey(z => z.TicketId);

            builder.Entity<TicketInOrder>()
                .HasOne(z => z.UserOrder)
                .WithMany(z => z.Tickets)
                .HasForeignKey(z => z.OrderId);
        }
    }
}
