using Microsoft.AspNetCore.Mvc;
using YourProjectName.Models; // Import your models

public class AccountController : Controller
{
    // GET: /Account/Login
    // This simply displays the page
    public IActionResult Login()
    {
        return View();
    }

    // POST: /Account/Login
    // This runs when the user clicks the "Log In" button
    [HttpPost]
    public IActionResult Login(LoginViewModel model)
    {
        // We aren't doing logic yet, so we will just redirect
        // back to the home page for now so the form "works".
        return RedirectToAction("Dashboard", "Books");
    }
}