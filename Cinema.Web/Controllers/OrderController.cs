using Cinema.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Cinema.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        public IActionResult Index()
        {
            //show all customers orders
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orders = _orderService.getAllOrders(userId);
            return View(orders);
        }

        public IActionResult Details(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = _orderService.getDetailsForOrder(id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);

        }
    }
}
