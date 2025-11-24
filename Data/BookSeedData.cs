using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MvcMovie.Data;
using MvcMovie.Models;
using System;
using System.Linq;

namespace MvcMovie.Data
{
    public static class BookSeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new MvcMovieContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<MvcMovieContext>>());

            // Make sure DB exists + migrations applied
            context.Database.Migrate();

            // Skip if already seeded
            if (context.Books.Any())
                return;

            context.Books.AddRange(
                new Book
                {
                    Title = "Pride and Prejudice",
                    Author = "Jane Austen",
                    ReleaseDate = new DateTime(1813, 1, 28),
                    Genre = "Classic Romance",
                    TotalPages = 432,
                    CurrentPage = 432,
                    Status = "Completed",
                    Rating = 5
                },
                new Book
                {
                    Title = "1984",
                    Author = "George Orwell",
                    ReleaseDate = new DateTime(1949, 6, 8),
                    Genre = "Dystopian",
                    TotalPages = 328,
                    CurrentPage = 328,
                    Status = "Completed",
                    Rating = 5
                },
                new Book
                {
                    Title = "To Kill a Mockingbird",
                    Author = "Harper Lee",
                    ReleaseDate = new DateTime(1960, 7, 11),
                    Genre = "Classic",
                    TotalPages = 281,
                    CurrentPage = 281,
                    Status = "Completed",
                    Rating = 5
                },
                new Book
                {
                    Title = "The Great Gatsby",
                    Author = "F. Scott Fitzgerald",
                    ReleaseDate = new DateTime(1925, 4, 10),
                    Genre = "Classic",
                    TotalPages = 180,
                    CurrentPage = 180,
                    Status = "Completed",
                    Rating = 4
                },
                new Book
                {
                    Title = "Moby-Dick",
                    Author = "Herman Melville",
                    ReleaseDate = new DateTime(1851, 10, 18),
                    Genre = "Adventure",
                    TotalPages = 635,
                    CurrentPage = 120,
                    Status = "Reading",
                    Rating = 4
                },
                new Book
                {
                    Title = "Jane Eyre",
                    Author = "Charlotte Brontë",
                    ReleaseDate = new DateTime(1847, 10, 16),
                    Genre = "Gothic",
                    TotalPages = 500,
                    CurrentPage = 0,
                    Status = "To Read",
                    Rating = null
                },
                new Book
                {
                    Title = "The Hobbit",
                    Author = "J.R.R. Tolkien",
                    ReleaseDate = new DateTime(1937, 9, 21),
                    Genre = "Fantasy",
                    TotalPages = 310,
                    CurrentPage = 150,
                    Status = "Reading",
                    Rating = 5
                },
                new Book
                {
                    Title = "Crime and Punishment",
                    Author = "Fyodor Dostoevsky",
                    ReleaseDate = new DateTime(1866, 1, 1),
                    Genre = "Psychological",
                    TotalPages = 671,
                    CurrentPage = 0,
                    Status = "To Read",
                    Rating = null
                },
                new Book
                {
                    Title = "The Catcher in the Rye",
                    Author = "J.D. Salinger",
                    ReleaseDate = new DateTime(1951, 7, 16),
                    Genre = "Classic",
                    TotalPages = 277,
                    CurrentPage = 277,
                    Status = "Completed",
                    Rating = 4
                },
                new Book
                {
                    Title = "Wuthering Heights",
                    Author = "Emily Brontë",
                    ReleaseDate = new DateTime(1847, 12, 1),
                    Genre = "Gothic",
                    TotalPages = 416,
                    CurrentPage = 80,
                    Status = "Reading",
                    Rating = 3
                }
            );

            context.SaveChanges();
        }
    }
}
