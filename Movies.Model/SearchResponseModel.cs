using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Model
{
    public class SearchResponseModel
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public int YearOfRelease { get; set; }
        public int RunningTime { get; set; }
        public string Genres { get; set; }
        public decimal AverageRating { get; set; }
    }
}
