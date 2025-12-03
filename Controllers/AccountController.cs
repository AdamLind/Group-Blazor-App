using Microsoft.AspNetCore.Mvc;
using YourProjectName.Models; // keep this as-is if your viewmodels use this namespace

public class AccountController : Controller
{
    // GET: /Account/Login
    public IActionResult Login()
    {
        return View();
    }

    // POST: /Account/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Temporary: just redirect so the form "works"
        return RedirectToAction("Dashboard", "Books");
    }

    // GET: /Account/Register
    public IActionResult Register()
    {
        return View();
    }

    // POST: /Account/Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Later you’ll add real user creation here

        // For now, after "registering", send them to Login
        return RedirectToAction("Login");
    }
}
