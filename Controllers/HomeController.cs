using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DebuggingChallenge.Models;

namespace DebuggingChallenge.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("user/create")]
        public IActionResult CreateUser(User newUser)
        {
            if (ModelState.IsValid)
            {
                HttpContext.Session.SetString("Username", newUser.Name);
                if (newUser.Location != null)
                {
                    HttpContext.Session.SetString("Location", newUser.Location);
                }
                else
                {
                    HttpContext.Session.SetString("Location", "Undisclosed");
                }
                GeneratePasscode();
                return RedirectToAction("Generator");
            }
            else
            {
                return View("Index");
            }
        }

        [HttpGet("generator")]
        public IActionResult Generator()
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Index");
            }
            if (HttpContext.Session.GetString("Passcode") == null)
            {
                GeneratePasscode();
            }
            string username = HttpContext.Session.GetString("Username");
            string location = HttpContext.Session.GetString("Location");
            string passcode = HttpContext.Session.GetString("Passcode");
            ViewBag.Username = username;
            ViewBag.Location = location;
            ViewBag.Passcode = passcode;
            return View();
        }

        [HttpPost("reset")]
        public IActionResult Reset()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        [HttpPost("generate/new")]
        public IActionResult GenerateNew()
        {
            GeneratePasscode();
            return RedirectToAction("Generator");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        public void GeneratePasscode()
        {
            string passcode = "";
            string CharOptions = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string NumOptions = "0123456789";
            Random rand = new Random();
            for (int i = 0; i < 15; i++)
            {
                int odds = rand.Next(2);
                if (odds == 0)
                {
                    passcode += CharOptions[rand.Next(CharOptions.Length)];
                }
                else
                {
                    passcode += NumOptions[rand.Next(NumOptions.Length)];
                }
            }
            HttpContext.Session.SetString("Passcode", passcode);
        }
    }
}
