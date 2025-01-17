﻿using Cinema.Domain.DomainModels;
using Cinema.Domain.DTO;
using Cinema.Domain.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Cinema.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<CinemaApplicationUser> userManager;
        private readonly SignInManager<CinemaApplicationUser> signInManager;
        public AccountController(UserManager<CinemaApplicationUser> userManager,
            SignInManager<CinemaApplicationUser> signInManager)
        {

            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpGet, AllowAnonymous]
        public IActionResult Register()
        {
            UserRegistrationDTO model = new UserRegistrationDTO();
            return View(model);
        }
        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Register(UserRegistrationDTO request)
        {
            
            if (ModelState.IsValid)
            {
                var userCheck = await userManager.FindByEmailAsync(request.Email);
                if (userCheck == null)
                {
                    var user = new CinemaApplicationUser
                    {
                        UserName = request.Email,
                        NormalizedUserName = request.Email,
                        Email = request.Email,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                        isAdmin = false,
                        UserCart = new ShoppingCart()
                    };
                    var result = await userManager.CreateAsync(user, request.Password);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        if (result.Errors.Count() > 0)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("message", error.Description);
                            }
                        }
                        return View(request);
                    }
                }
                else
                {
                    ModelState.AddModelError("message", "Email already exists.");
                    return View(request);
                }
            }
            return View(request);

        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            UserLoginDTO model = new UserLoginDTO();
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginDTO model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null && !user.EmailConfirmed)
                {
                    ModelState.AddModelError("message", "Email not confirmed yet");
                    return View(model);

                }
                if (await userManager.CheckPasswordAsync(user, model.Password) == false)
                {
                    ModelState.AddModelError("message", "Invalid credentials");
                    return View(model);

                }

                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, true);

                if (result.Succeeded)
                {
                    await userManager.AddClaimAsync(user, new Claim("UserRole", "Admin"));
                    return RedirectToAction("Index", "Tickets");
                }
                else if (result.IsLockedOut)
                {
                    return View("AccountLocked");
                }
                else
                {
                    ModelState.AddModelError("message", "Invalid login attempt");
                    return View(model);
                }
            }
            return View(model);
        }


        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
        public IActionResult Edit(Guid? id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            CinemaApplicationUser currentUser = userManager.FindByIdAsync(userId.ToString()).Result;
            if (currentUser.isAdmin)
            {
                if (id == null)
                {
                    return NotFound();
                }
                var user = userManager.FindByIdAsync(id.ToString()).Result;


                if (user == null)
                {
                    return NotFound();
                }
                return View(user);
            }
            else return StatusCode(403);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("isAdmin")] CinemaApplicationUser user)
        {
            
            CinemaApplicationUser currentUser=userManager.FindByIdAsync(id.ToString()).Result;
            if (user.isAdmin)
            {
                currentUser.isAdmin = true;
            }
            else currentUser.isAdmin = false;

            await userManager.UpdateAsync(currentUser);

            return RedirectToAction("Index");
        }

        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            CinemaApplicationUser currentUser = userManager.FindByIdAsync(userId.ToString()).Result;
            if (currentUser.isAdmin)
            {
                List<CinemaApplicationUser> users = userManager.Users.ToList();
                return View(users);
            }
            else return StatusCode(403);
         
        }
        public IActionResult AdminDashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            CinemaApplicationUser currentUser = userManager.FindByIdAsync(userId.ToString()).Result;
            if (currentUser.isAdmin)
                return View();
            else return StatusCode(403);
        }
    }
}
