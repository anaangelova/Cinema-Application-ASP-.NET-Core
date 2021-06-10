using Cinema.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cinema.Repository.Interface
{
    public interface IOrderRepository
    {
        List<Order> getAllOrders(string userId);
        Order getOrderDetails(Guid id);
      
    }
}
