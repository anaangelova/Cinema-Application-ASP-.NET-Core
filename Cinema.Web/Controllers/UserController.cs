using Cinema.Domain.DomainModels;
using Cinema.Domain.Identity;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Cinema.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<CinemaApplicationUser> userManager;

        public UserController(UserManager<CinemaApplicationUser> _userManager)
        {
            userManager = _userManager;
        }
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            CinemaApplicationUser currentUser = userManager.FindByIdAsync(userId.ToString()).Result;
            if (currentUser.isAdmin)
                return View();
            else return StatusCode(403);
        }
        public IActionResult ImportUsers(IFormFile file)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            CinemaApplicationUser currentUser = userManager.FindByIdAsync(userId.ToString()).Result;
            if (currentUser.isAdmin)
            {
                string pathToUpload = $"{Directory.GetCurrentDirectory()}\\files\\{file.FileName}";

                using (FileStream fileStream = System.IO.File.Create(pathToUpload))
                {
                    file.CopyTo(fileStream);

                    fileStream.Flush();
                }

                List<UserModel> users = getAllUsersFromFile(file.FileName);
                //tuka gi imame korisnicite koi sega treba da gi dodademe vo bazata
                bool status = true;
                foreach (var item in users)
                {
                    var userCheck = userManager.FindByEmailAsync(item.Email).Result;
                    if (userCheck == null)
                    {
                        bool isAdminUser = true ? item.Role.Equals("Admin") : false;
                        var user = new CinemaApplicationUser
                        {
                            UserName = item.Email,
                            NormalizedUserName = item.Email,
                            Email = item.Email,
                            EmailConfirmed = true,
                            PhoneNumberConfirmed = true,
                            isAdmin = isAdminUser,
                            UserCart = new ShoppingCart()
                        };
                        var result = userManager.CreateAsync(user, item.Password).Result;
                        status = status && result.Succeeded;
                    }

                }
                if (status)
                {
                    return RedirectToAction("Index", "Tickets");
                }
                else return RedirectToAction("Index", "Tickets");
            }
           else return StatusCode(403);

        }

        private List<UserModel> getAllUsersFromFile(string fileName)
        {
            List<UserModel> users = new List<UserModel>();

            string filePath = $"{Directory.GetCurrentDirectory()}\\files\\{fileName}";

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        users.Add(new UserModel
                        {
                            Email=reader.GetValue(0).ToString(),
                            Password= reader.GetValue(1).ToString(),
                            Role=reader.GetValue(2).ToString()
                        });
                    }
                }
            }

            return users;
        }
    }
}
