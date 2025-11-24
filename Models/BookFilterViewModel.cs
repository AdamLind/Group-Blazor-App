using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace MvcMovie.Models;

public class BookFilterViewModel
{
    public List<Book>? Books { get; set; }

    public SelectList? Genres { get; set; }
    public SelectList? Authors { get; set; }

    public string? SelectedGenre { get; set; }
    public string? SelectedAuthor { get; set; }
    public int? ReleaseYear { get; set; }

    // optional: keep title search like movies
    public string? TitleSearch { get; set; }
}
