using System.Collections.Generic;

namespace MvcMovie.Models
{
    public class ReadingDashboardViewModel
    {
        public int TotalBooks { get; set; }
        public int ToReadCount { get; set; }
        public int ReadingCount { get; set; }
        public int CompletedCount { get; set; }

        public int TotalPagesRead { get; set; }
        public int TotalPagesPlanned { get; set; }
        public double OverallCompletionPercent { get; set; }

        public double AverageReadingCompletionPercent { get; set; }

        public List<Book> CurrentlyReading { get; set; } = new();
    }
}
