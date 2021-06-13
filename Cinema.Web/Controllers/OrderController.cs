using Cinema.Services.Interface;
using GemBox.Document;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
            ComponentInfo.SetLicense("FREE-LIMITED-KEY");
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

        public FileContentResult CreateInvoice(Guid? id)
        {
            var order = _orderService.getDetailsForOrder(id.Value);
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Invoice.docx");
            var document = DocumentModel.Load(templatePath);
            document.Content.Replace("{{OrderNumber}}", order.Id.ToString());
            document.Content.Replace("{{Username}}", order.User.UserName);

            StringBuilder sb = new StringBuilder();

            var totalPrice = 0.0;

            foreach (var item in order.Tickets)
            {
                totalPrice += item.Quantity * item.SelectedTicket.TicketPrice;
                sb.AppendLine(item.SelectedTicket.TicketName + " with quantity of: " + item.Quantity + " and price of: " + item.SelectedTicket.TicketPrice + "MKD");
            }


            document.Content.Replace("{{TicketList}}", sb.ToString());
            document.Content.Replace("{{TotalPrice}}", totalPrice.ToString() + "MKD");

            var stream = new MemoryStream();

            document.Save(stream, new PdfSaveOptions());

            return File(stream.ToArray(), new PdfSaveOptions().ContentType, "ExportInvoice_"+order.Id+".pdf");
        }
    }
}
