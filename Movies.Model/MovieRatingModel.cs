using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Model
{
    public class MovieRatingModel
    {
        public long MovieId { get; set; }
        public long CustomerId { get; set; }
        public decimal Rating { get; set; }
    }
}
