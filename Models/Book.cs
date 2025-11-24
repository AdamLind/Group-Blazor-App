using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcMovie.Models;

public class Book
{
    public int Id { get; set; }

    [StringLength(120, MinimumLength = 1)]
    [Required]
    public string? Title { get; set; }

    [StringLength(80, MinimumLength = 1)]
    [Required]
    public string? Author { get; set; }

    [Display(Name = "Release Date")]
    [DataType(DataType.Date)]
    public DateTime ReleaseDate { get; set; }

    [RegularExpression(@"^[A-Z]+[a-zA-Z\s]*$")]
    [Required]
    [StringLength(30)]
    public string? Genre { get; set; }

    [Display(Name = "Total Pages")]
    [Range(1, 10000)]
    public int TotalPages { get; set; }

    [Display(Name = "Current Page")]
    [Range(0, 10000)]
    public int CurrentPage { get; set; }

    // e.g. To Read, Reading, Completed
    [Required]
    [StringLength(20)]
    public string? Status { get; set; }

    [Range(1, 5)]
    [Display(Name = "Rating (1–5)")]
    public int? Rating { get; set; }

    [NotMapped]
    [Display(Name = "Completion %")]
    public double CompletionPercent =>
        TotalPages > 0 ? Math.Round((double)CurrentPage / TotalPages * 100, 1) : 0;
}
