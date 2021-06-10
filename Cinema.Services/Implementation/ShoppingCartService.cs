using Cinema.Domain.DomainModels;
using Cinema.Domain.DTO;
using Cinema.Repository.Interface;
using Cinema.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinema.Services.Implementation
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRepository<ShoppingCart> _shoppingCartRepository;
        private readonly IRepository<Order> _orderRepositorty;
        private readonly IRepository<TicketInOrder> _productInOrderRepositorty;

        public ShoppingCartService(IUserRepository userRepository, IRepository<ShoppingCart> shoppingCartRepository, IRepository<Order> orderRepository, IRepository<TicketInOrder> productInOrderRepositorty)
        {
            _userRepository = userRepository;
            _shoppingCartRepository = shoppingCartRepository;
            _orderRepositorty = orderRepository;
            _productInOrderRepositorty = productInOrderRepositorty;
        }
        public bool deleteProductFromCart(Guid productId, string userId)
        {
            if (!string.IsNullOrEmpty(userId) && productId != null)
            {
                var loggedUser = _userRepository.Get(userId);

                var userShoppingCart = loggedUser.UserCart;


                var productToRemove = userShoppingCart.TicketsInShoppingCart.Where(z => z.TicketId.Equals(productId)).FirstOrDefault();
                userShoppingCart.TicketsInShoppingCart
                    .Remove(productToRemove);

                _shoppingCartRepository.Update(userShoppingCart);
                return true;
            }
            return false;
        }

        public ShoppingCartDTO getShoppingCartInfo(string userId)
        {
            var loggedUser = _userRepository.Get(userId);


            var userShoppingCart = loggedUser.UserCart;

            var productsPriceQuantity = userShoppingCart.TicketsInShoppingCart.Select(z => new
            {
                ProductPrice = z.Ticket.TicketPrice,
                Quantity = z.TicketQuantity
            }).ToList();

            double totalPrice = 0;
            foreach (var item in productsPriceQuantity)
            {
                totalPrice += item.ProductPrice * item.Quantity;

            }


            ShoppingCartDTO itemToSendToView = new ShoppingCartDTO
            {
                TicketInShoppingCart = userShoppingCart.TicketsInShoppingCart.ToList(),
                TotalPrice = totalPrice
            };
            return itemToSendToView;
        }

        public bool orderNow(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                //Select * from Users Where Id LIKE userId

                var loggedInUser = this._userRepository.Get(userId);

                var userShoppingCart = loggedInUser.UserCart;

               
                Order order = new Order
                {
                    Id = Guid.NewGuid(),
                    User = loggedInUser,
                    UserId = userId
                };

                this._orderRepositorty.Insert(order);

                List<TicketInOrder> productInOrders = new List<TicketInOrder>();

                var result = userShoppingCart.TicketsInShoppingCart.Select(z => new TicketInOrder
                {
                    Id = Guid.NewGuid(),
                    TicketId = z.Ticket.Id,
                    SelectedTicket = z.Ticket,
                    OrderId = order.Id,
                    UserOrder = order
                }).ToList();

                StringBuilder sb = new StringBuilder();

                var totalPrice = 0;

                sb.AppendLine("Your order is completed. The order conains: ");

                for (int i = 1; i <= result.Count(); i++)
                {
                    var item = result[i - 1];

                    totalPrice += item.Quantity * item.SelectedTicket.TicketPrice;

                    sb.AppendLine(i.ToString() + ". " + item.SelectedTicket.TicketName + " with price of: " + item.SelectedTicket.TicketPrice + " and quantity of: " + item.Quantity);
                }

                sb.AppendLine("Total price: " + totalPrice.ToString());

                productInOrders.AddRange(result);

                foreach (var item in productInOrders)
                {
                    this._productInOrderRepositorty.Insert(item);
                }

                loggedInUser.UserCart.TicketsInShoppingCart.Clear();

                this._userRepository.Update(loggedInUser);
               

                return true;
            }
            return false;
        }
    }

        //order metoda treba da se impl.
    
}
