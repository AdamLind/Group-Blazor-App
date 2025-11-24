using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcMovie.Data;
using MvcMovie.Models;

namespace MvcMovie.Controllers
{
    public class BooksController : Controller
    {
        private readonly MvcMovieContext _context;

        public BooksController(MvcMovieContext context)
        {
            _context = context;
        }

        // GET: Books
        public async Task<IActionResult> Index(
            string? selectedGenre,
            string? selectedAuthor,
            string? titleSearch,
            int? releaseYear)
        {
            // base query
            var booksQuery = _context.Books.AsQueryable();

            // Filters
            if (!string.IsNullOrWhiteSpace(titleSearch))
            {
                booksQuery = booksQuery.Where(b =>
                    b.Title!.ToUpper().Contains(titleSearch.ToUpper()));
            }

            if (!string.IsNullOrWhiteSpace(selectedGenre))
            {
                booksQuery = booksQuery.Where(b => b.Genre == selectedGenre);
            }

            if (!string.IsNullOrWhiteSpace(selectedAuthor))
            {
                booksQuery = booksQuery.Where(b => b.Author == selectedAuthor);
            }

            if (releaseYear.HasValue)
            {
                booksQuery = booksQuery.Where(b => b.ReleaseDate.Year == releaseYear.Value);
            }

            // For dropdowns
            var genreQuery = _context.Books
                .OrderBy(b => b.Genre)
                .Select(b => b.Genre)
                .Distinct();

            var authorQuery = _context.Books
                .OrderBy(b => b.Author)
                .Select(b => b.Author)
                .Distinct();

            var vm = new BookFilterViewModel
            {
                Books = await booksQuery.ToListAsync(),
                Genres = new SelectList(await genreQuery.ToListAsync()),
                Authors = new SelectList(await authorQuery.ToListAsync()),
                SelectedGenre = selectedGenre,
                SelectedAuthor = selectedAuthor,
                ReleaseYear = releaseYear,
                TitleSearch = titleSearch
            };

            return View(vm);
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            // Default status "Reading" just so the dropdown has something
            var model = new Book
            {
                Status = "To Read",
                ReleaseDate = DateTime.Today
            };
            return View(model);
        }

        // POST: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Author,ReleaseDate,Genre,TotalPages,CurrentPage,Status,Rating")] Book book)
        {
            // simple guard: current cannot exceed total
            if (book.CurrentPage > book.TotalPages)
            {
                ModelState.AddModelError(nameof(book.CurrentPage), "Current page cannot be greater than total pages.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: Books/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Author,ReleaseDate,Genre,TotalPages,CurrentPage,Status,Rating")] Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (book.CurrentPage > book.TotalPages)
            {
                ModelState.AddModelError(nameof(book.CurrentPage), "Current page cannot be greater than total pages.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Books == null)
            {
                return Problem("Entity set 'MvcMovieContext.Books'  is null.");
            }
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return (_context.Books?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
