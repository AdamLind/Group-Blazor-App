using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims; // For finding the User ID
using MvcMovie.Data;
using MvcMovie.Models;

namespace MvcMovie.Controllers
{
    [Authorize]
    public class BooksController : Controller
    {
        private readonly MVCBookContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public BooksController(MVCBookContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // --------------------------
        // LIST + FILTERS
        // --------------------------
        public async Task<IActionResult> Index(
            string? selectedGenre,
            string? selectedAuthor,
            string? selectedStatus,
            string? titleSearch,
            int? releaseYear)
        {

            var currentUserId = _userManager.GetUserId(User);

            var booksQuery = _context.Books
                                     .Where(b => b.OwnerId == currentUserId)
                                     .AsQueryable();

            if (!string.IsNullOrWhiteSpace(titleSearch))
                booksQuery = booksQuery.Where(b =>
                    b.Title!.ToUpper().Contains(titleSearch.ToUpper()));

            if (!string.IsNullOrWhiteSpace(selectedGenre))
                booksQuery = booksQuery.Where(b => b.Genre == selectedGenre);

            if (!string.IsNullOrWhiteSpace(selectedAuthor))
                booksQuery = booksQuery.Where(b => b.Author == selectedAuthor);

            if (!string.IsNullOrWhiteSpace(selectedStatus))
                booksQuery = booksQuery.Where(b => b.Status == selectedStatus);

            if (releaseYear.HasValue)
                booksQuery = booksQuery.Where(b => b.ReleaseDate.Year == releaseYear.Value);

            var vm = new BookFilterViewModel
            {
                Books = await booksQuery.ToListAsync(),
                Genres = new SelectList(await _context.Books.Select(b => b.Genre).Distinct().ToListAsync()),
                Authors = new SelectList(await _context.Books.Select(b => b.Author).Distinct().ToListAsync()),
                Statuses = new SelectList(await _context.Books.Select(b => b.Status).Distinct().ToListAsync()),
                SelectedGenre = selectedGenre,
                SelectedAuthor = selectedAuthor,
                SelectedStatus = selectedStatus,
                ReleaseYear = releaseYear,
                TitleSearch = titleSearch
            };

            return View(vm);
        }

        // --------------------------
        // DASHBOARD
        // --------------------------
        public async Task<IActionResult> Dashboard()
        {
            var currentUserId = _userManager.GetUserId(User);

            var books = await _context.Books
                                      .Where(b => b.OwnerId == currentUserId)
                                      .ToListAsync();

            if (!books.Any())
                return View(new ReadingDashboardViewModel());

            var toRead = books.Count(b => b.Status == "To Read");
            var reading = books.Count(b => b.Status == "Reading");
            var completed = books.Count(b => b.Status == "Completed");

            var totalPagesRead = books.Sum(b => Math.Min(b.CurrentPage, b.TotalPages));
            var totalPagesPlanned = books.Sum(b => b.TotalPages);

            double overallCompletion = totalPagesPlanned > 0
                ? Math.Round((double)totalPagesRead / totalPagesPlanned * 100, 1)
                : 0;

            var readingBooks = books.Where(b => b.Status == "Reading").ToList();

            double avgReadingCompletion = readingBooks.Any()
                ? Math.Round(readingBooks.Average(b => b.CompletionPercent), 1)
                : 0;

            var vm = new ReadingDashboardViewModel
            {
                TotalBooks = books.Count,
                ToReadCount = toRead,
                ReadingCount = reading,
                CompletedCount = completed,
                TotalPagesRead = totalPagesRead,
                TotalPagesPlanned = totalPagesPlanned,
                OverallCompletionPercent = overallCompletion,
                AverageReadingCompletionPercent = avgReadingCompletion,
                CurrentlyReading = readingBooks
            };

            return View(vm);
        }

        // --------------------------
        // CRUD
        // --------------------------

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books.FirstOrDefaultAsync(m => m.Id == id);
            if (book == null) return NotFound();

            // --- SECURITY CHECK ---
            var currentUserId = _userManager.GetUserId(User);
            if (book.OwnerId != currentUserId) 
            {
                return Forbid();
            }
            // ----------------------

            return View(book);
        }

        public IActionResult Create()
        {
            return View(new Book
            {
                Status = "To Read",
                ReleaseDate = DateTime.Today
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book book)
        {
            if (book.CurrentPage > book.TotalPages)
                ModelState.AddModelError(nameof(book.CurrentPage), "Current page cannot exceed total pages.");

            if (ModelState.IsValid)
            {
                book.OwnerId = _userManager.GetUserId(User);

                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            // --- SECURITY CHECK ---
            var currentUserId = _userManager.GetUserId(User);
            if (book.OwnerId != currentUserId) 
            {
                return Forbid();
            }
            // ----------------------

            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Book book)
        {
            if (id != book.Id) return NotFound();

            if (book.CurrentPage > book.TotalPages)
                ModelState.AddModelError(nameof(book.CurrentPage), "Current page cannot exceed total pages.");

            if (ModelState.IsValid)
            {
                try
                {
                    var userId = _userManager.GetUserId(User);
                    book.OwnerId = userId; 
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books.FirstOrDefaultAsync(m => m.Id == id);
            if (book == null) return NotFound();

            // --- SECURITY CHECK ---
            var currentUserId = _userManager.GetUserId(User);
            if (book.OwnerId != currentUserId) 
            {
                return Forbid();
            }
            // ----------------------

            return View(book);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
                _context.Books.Remove(book);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}
