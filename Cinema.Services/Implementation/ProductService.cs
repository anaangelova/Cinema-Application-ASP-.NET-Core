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
    public class ProductService : IProductService
    {

        private readonly IRepository<Ticket> _productRepository;
        private readonly IRepository<TicketInShoppingCart> _productInShoppingCartRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITicketRepository _ticketRepository;

        public ProductService(IRepository<Ticket> productRepository, IRepository<TicketInShoppingCart> productInShoppingCartRepository, IUserRepository userRepository, ITicketRepository ticketRepository)
        {
            _productRepository = productRepository;

            _productInShoppingCartRepository = productInShoppingCartRepository;
            _userRepository = userRepository;
            _ticketRepository = ticketRepository;
        }
        public bool AddToShoppingCart(AddToShoppingCartDTO item, string userID)
        {
            var user = this._userRepository.Get(userID);

            var userShoppingCard = user.UserCart;

            if (item.TicketId != null && userShoppingCard != null)
            {
                var product = this.GetDetailsForProduct(item.TicketId);

                if (product != null)
                {
                    TicketInShoppingCart itemToAdd = new TicketInShoppingCart
                    {
                        Id = Guid.NewGuid(),
                        Ticket = product,
                        TicketId = product.Id,
                        ShoppingCart = userShoppingCard,
                        ShoppingCartId = userShoppingCard.Id,
                        TicketQuantity = item.TicketQuantity
                    };

                    this._productInShoppingCartRepository.Insert(itemToAdd);

                    return true;
                }
                return false;
            }

            return false;
        }

        public void CreateNewProduct(Ticket p)
        {
            this._productRepository.Insert(p);
        }

        public void DeleteProduct(Guid id)
        {
            var product = this.GetDetailsForProduct(id);
            this._productRepository.Delete(product);
        }

        public List<Ticket> GetAllProducts()
        {
            return this._productRepository.GetAll().ToList();
        }

        public Ticket GetDetailsForProduct(Guid? id)
        {
            return this._productRepository.Get(id);
        }

        public AddToShoppingCartDTO GetShoppingCartInfo(Guid? id)
        {
            var product = this.GetDetailsForProduct(id);
            AddToShoppingCartDTO model = new AddToShoppingCartDTO
            {
                SelectedTicket = product,
                TicketId = product.Id,
                TicketQuantity = 1
            };

            return model;
        }

        public void UpdeteExistingProduct(Ticket p)
        {
            this._productRepository.Update(p);
        }

        public IEnumerable<Ticket> getFilteredTickets(DateTime? start, DateTime? end)
        {
            return _ticketRepository.GetFilteredTickets(start, end);
        }
    }
}
