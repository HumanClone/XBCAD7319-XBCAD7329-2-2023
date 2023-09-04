using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MVCAPP.Data;
using MVCAPP.Models;

namespace MVCAPP.Controllers
{
    public class UserLoginController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserLoginController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(UserLogin userLogin)
        {
            return View();
        }
    }
}
