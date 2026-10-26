using Domain.Entities;
using Domain.Aggregates;
﻿using Domain.DTOs;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServicesClasseslibrary.Interface;
using System;
using Microsoft.AspNetCore.Authorization;

namespace OnlineExamSystem.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
       
        IUserService _userService;
        public AuthController(IUserService userService)
        {

            _userService = userService;
        }
      
        public IActionResult Login() => View(LoginDTO.Define());
       
        [HttpPost]
        public IActionResult Login(LoginDTO m)
        {
            
            m.ChangePassword(Hash(m.Password));
            var userValid =///call userSerivces
                _userService.Validate(m);
            if (userValid!=null)
            {
                HttpContext.Session.SetString("Role", userValid.Role);
                HttpContext.Session.SetInt32("UserId", userValid.Id);
                return RedirectToAction("Index", userValid.Role == "Examiner" ? "Examiner" : "Home");
            }
            ModelState.AddModelError("", "Invalid"); return View(m);
        }
        public IActionResult Register() => View();
        [HttpPost]
        public IActionResult Register(RegisterDTO m)
        {
            m.ChangePassword(Hash(m.Password));
            m.ChangeRole("Examiner");
            _userService.Add(m);
            return RedirectToAction("Login");
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
        private string Hash(string password)
        {
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(password);
                var hash = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
   
  }
