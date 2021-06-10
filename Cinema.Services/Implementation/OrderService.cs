using Cinema.Domain.DomainModels;
using Cinema.Repository.Interface;
using Cinema.Services.Interface;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Cinema.Services.Implementation
{
    public class OrderService : IOrderService
    {
        private readonly IUserRepository userRepository;
        private readonly IOrderRepository orderRepository;

        public OrderService(IUserRepository _userRepository, IOrderRepository _orderRepository)
        {
            userRepository = _userRepository;
            orderRepository = _orderRepository;
        }
        public List<Order> getAllOrders(string userId)
        {
            return orderRepository.getAllOrders(userId);
        }

        public Order getDetailsForOrder(Guid id)
        {
            return orderRepository.getOrderDetails(id);
        }
    }
}
