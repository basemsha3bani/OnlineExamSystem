using DataModel;
using DataRepository.DataRepositoryEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServicesClasseslibrary.Interface;
using System;

namespace OnlineExamSystem.Controllers
{
    public class AuthController : Controller
    {
        IUserService _userService;
        public AuthController(IUserService userService)
        {

            _userService = userService;
        }
      
        public IActionResult Login() => View(new LoginModel());
        [HttpPost]
        public IActionResult Login(LoginModel m)
        {
            m.Password = Hash(m.Password);
            var userValid =///call userSerivces
                _userService.Validate(m);
            if (userValid!=null)
            {
                HttpContext.Session.SetString("Role", userValid.Role);
                HttpContext.Session.SetInt32("UserId", userValid.Id);
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("", "Invalid"); return View(m);
        }
        public IActionResult Register() => View();
        [HttpPost]
        public IActionResult Register(RegisterModel m)
        {
            m.Password = Hash(m.Password);
            m.Role = "Examiner";
            _userService.Add(m);
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
