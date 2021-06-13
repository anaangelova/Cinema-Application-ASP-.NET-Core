using Cinema.Domain.DomainModels;
using Cinema.Domain.DTO;
using Cinema.Domain.Identity;
using Cinema.Services.Interface;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Cinema.Web.Controllers
{
    public class TicketsController : Controller
    {
        private readonly IProductService _productService;
        private readonly UserManager<CinemaApplicationUser> _userManager;
        public TicketsController(IProductService productService, UserManager<CinemaApplicationUser> userManager)
        {
            _productService = productService;
            _userManager = userManager;

        }
        public IActionResult Index()
        {
            var allProducts = this._productService.GetAllProducts();
            return View(allProducts);
        }

        // GET: Tickets/Details/5
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = this._productService.GetDetailsForProduct(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Tickets/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,TicketName,TicketDescription,TicketGenre,TicketPrice,TicketImage,TicketDate")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                this._productService.CreateNewProduct(ticket);
                return RedirectToAction(nameof(Index));
            }
            return View(ticket);
        }
        // GET: Tickets/Edit/5
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = this._productService.GetDetailsForProduct(id);

            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, [Bind("Id,TicketName,TicketDescription,TicketGenre,TicketPrice,TicketImage,TicketDate")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    this._productService.UpdeteExistingProduct(ticket);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = this._productService.GetDetailsForProduct(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {

            this._productService.DeleteProduct(id);
            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(Guid id)
        {
            return this._productService.GetDetailsForProduct(id) != null;
        }

        //GET Products/AddProductToCard
        public IActionResult AddTicketToCard(Guid? id)
        {

            var model = this._productService.GetShoppingCartInfo(id);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddTicketToCard([Bind("TicketId", "TicketQuantity")] AddToShoppingCartDTO item)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = this._productService.AddToShoppingCart(item, userId);

            if (result)
            {
                return RedirectToAction("Index", "Tickets");
            }

            return View(item);
        }

        public IActionResult FilterTickets(DateTime? start, DateTime? end) //ovaa e moja dopolnitelna akcija
        {

            var tickets = _productService.getFilteredTickets(start, end);
            return View("Index", tickets);

        }

        public IActionResult ExportTickets()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            CinemaApplicationUser currentUser = _userManager.FindByIdAsync(userId.ToString()).Result;
            if (currentUser.isAdmin)
            {
                //prvo treba da gi zememe site postoechki zhanrovi 
                var allProducts = this._productService.GetAllProducts();
                List<string> genres = new List<string>();
                foreach (var ticket in allProducts)
                {
                    genres.Add(ticket.TicketGenre);

                }
                genres = genres.Distinct().ToList();
                TicketDTO ticketDTO = new TicketDTO();
                ticketDTO.Genres = new List<string>();
                ticketDTO.areChecked = new List<bool>();
                foreach (string genre in genres)
                {
                    ticketDTO.Genres.Add(genre);
                    ticketDTO.areChecked.Add(false);
                  
                }

                return View(ticketDTO);
            }
            else return StatusCode(403);
        }

        [HttpPost]
        public IActionResult ExportInExcel(TicketDTO dto)
        {
            string fileName = "Tickets.xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            List<string> selectedGenres = new List<string>();
            for(int i = 0; i < dto.Genres.Count; i++)
            {
                if (dto.areChecked[i])
                {
                    selectedGenres.Add(dto.Genres[i]);
                }
            }


            using (var workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet = workbook.Worksheets.Add("Filtered Tickets by Genre");
                worksheet.Cell(1, 1).Value = "Ticket Name";
                worksheet.Cell(1, 2).Value = "Ticket Genre";
                worksheet.Cell(1, 3).Value = "Ticket Price";
                worksheet.Cell(1, 4).Value = "Ticket Description";
                worksheet.Cell(1, 5).Value = "Ticket Date";

                List<Ticket> filtered = new List<Ticket>();
                filtered = _productService.GetTicketsByGenre(selectedGenres).ToList();
                for (int i = 1; i <= filtered.Count(); i++)
                {
                    var item = filtered[i - 1];

                    worksheet.Cell(i + 1, 1).Value = item.TicketName;
                    worksheet.Cell(i + 1, 2).Value = item.TicketGenre;
                    worksheet.Cell(i + 1, 3).Value = item.TicketPrice;
                    worksheet.Cell(i + 1, 4).Value = item.TicketDescription;
                    worksheet.Cell(i + 1, 5).Value = item.TicketDate.ToString();


                }
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(content, contentType, fileName);
                }

            }

        }

    }
}
