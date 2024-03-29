﻿using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebHashcatAdminPanel.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        
        private readonly string _defUsername = "admin";
        private readonly string _defPass = "!1Adminadminadmin";

        public HomeController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            if (!await _roleManager.RoleExistsAsync("Admin"))await _roleManager.CreateAsync(new IdentityRole("Admin"));

            if (await _userManager.FindByNameAsync(_defUsername) == null)
            {
                var admin = new User
                {
                    UserName = "admin",
                    Email = "admin@example.com"
                };

                var result = await _userManager.CreateAsync(admin, _defPass);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(admin, "Admin");

                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(admin);
                    await _userManager.ConfirmEmailAsync(admin, token);

                    return View();
                }
            }
            else
            {
                var admin = await _userManager.FindByNameAsync(_defUsername);
                if (await _userManager.CheckPasswordAsync(admin, _defPass)) return View();
            }

            return RedirectToAction("Index", "Login", new { Area = "Identity" });
        }
    }
}