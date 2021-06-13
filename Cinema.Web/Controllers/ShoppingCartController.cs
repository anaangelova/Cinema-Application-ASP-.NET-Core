using Cinema.Domain.DomainModels;
using Cinema.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Web.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IMailService _mailService;
        private readonly IUserService _userService;

        public ShoppingCartController(IShoppingCartService shoppingCartService, IMailService mailService, IUserService userService)
        {

            _shoppingCartService = shoppingCartService;
            _mailService = mailService;
            _userService = userService;

        }

        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return View(_shoppingCartService.getShoppingCartInfo(userId));

        }
        public IActionResult DeleteProductFromCart(Guid productId)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (_shoppingCartService.deleteProductFromCart(productId, userId))
                return RedirectToAction("Index");
            return RedirectToAction("Index");
        }

        public IActionResult PayOrder(string stripeEmail, string stripeToken)
        {
            var customerService = new CustomerService();
            var chargeService = new ChargeService();
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = this._shoppingCartService.getShoppingCartInfo(userId);
            var customer = customerService.Create(new CustomerCreateOptions
            {
                Email = stripeEmail,
                Source = stripeToken
            });

            var charge = chargeService.Create(new ChargeCreateOptions
            {
                Amount = (Convert.ToInt32(order.TotalPrice) * 100),
                Description = "Cinema Application Payment",
                Currency = "MKD",
                Customer = customer.Id
            });
            if (charge.Status == "succeeded")
            {
                var result = this.Order();

                if (result!=null)
                {
                    bool sent = this.sendMail(result).Result;

                    if(sent)                   
                    return RedirectToAction("Index", "ShoppingCart");
                    else return RedirectToAction("Index", "ShoppingCart");
                }
                else
                {
                    return RedirectToAction("Index", "ShoppingCart");
                }
            }

            return RedirectToAction("Index", "ShoppingCart");
        }
        private async Task<bool> sendMail(Domain.DomainModels.Order order)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _userService.getUser(userId);
            string mail = user.Email;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Your order contains the following tickets:");
            sb.AppendLine();
            var allTickets = order.Tickets.ToList();
            var totalPrice = 0.0;
            for(int i = 1; i < allTickets.Count(); i++)
            {
                var ticket = allTickets[i];
                sb.AppendLine(i.ToString() + ". " + ticket.SelectedTicket.TicketName + " with price of: " + ticket.SelectedTicket.TicketPrice + " and quantity of: " + ticket.Quantity);
                totalPrice += ticket.Quantity * ticket.SelectedTicket.TicketPrice;
            }
            sb.AppendLine();
            sb.AppendLine("The total price of your order is: " + totalPrice + "MKD");
            MailRequest sendMail = new MailRequest() {
                ToEmail = mail,
                Subject = "Order with ID: " + order.Id + " is completed!",
                Body = sb.ToString()
            };

            await _mailService.SendEmailAsync(sendMail);
            return true;

        }

        private Domain.DomainModels.Order Order()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = this._shoppingCartService.orderNow(userId);

            return result;
        }
    }
}
