using Cinema.Domain.DomainModels;
using Cinema.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cinema.Services.Interface
{
    public interface IShoppingCartService
    {
        public ShoppingCartDTO getShoppingCartInfo(string userId);
        public bool deleteProductFromCart(Guid productId, string userId);
        public Order orderNow(string userId);
    }
}
