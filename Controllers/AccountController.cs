using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MvcMovie.Models; 
using MvcMovie.Data;

public class AccountController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly MVCBookContext _context;

    public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, MVCBookContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    // GET: /Account/Register
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    // POST: /Account/Register
    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Create the user object
            var user = new IdentityUser { UserName = model.Username, Email = model.Email };
            
            // Try to save to database
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Sign them in immediately
                await _signInManager.SignInAsync(user, isPersistent: false);
                
                // Redirect to the home page (or Dashboard)
                return RedirectToAction("Index", ""); 
            }

            // If it failed (e.g. password too short), add errors to the form
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        // If we got here, something failed, redisplay form
        return View(model);
    }
    
    // GET: /Account/Login
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // POST: /Account/Login
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "");
            }
            
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        }
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        // 1. Clear the cookie
        await _signInManager.SignOutAsync();
        
        // 2. Send them back to the home page (or login page)
        return RedirectToAction("Index", ""); 
    }

    // GET: /Account/Delete
    [HttpGet]
    [Authorize] // Only logged-in users can see this
    public IActionResult Delete()
    {
        return View();
    }

    // POST: /Account/Delete
    [HttpPost]
    [Authorize]
    [ActionName("Delete")] // Calls this method when submitting to /Account/Delete
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed()
    {
        // 1. Get the current user
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        // 2. (Optional) Delete their books explicitly?
        // Entity Framework usually handles this via Cascade Delete if configured,
        // but to be safe, we can manually remove them first.
        var userBooks = _context.Books.Where(b => b.OwnerId == user.Id);
        _context.Books.RemoveRange(userBooks);
        
        // 3. Delete the user
        var result = await _userManager.DeleteAsync(user);

        if (result.Succeeded)
        {
            // 4. Sign them out (crucial!)
            await _signInManager.SignOutAsync();
            
            return RedirectToAction("Index", "Home");
        }

        // If it failed, show errors
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        return View();
    }
}