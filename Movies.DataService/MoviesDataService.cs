using Movies.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.DataService
{
    public class MoviesDataService
    {
        public static List<SearchResponseModel> GetSearchResult(string searchText)
        {
            ComcastEntities db = new ComcastEntities();
            List<SearchResponseModel> response = null;
          //  db.Database.Connection.Open();
            var result = (from movies in db.MOVIESTABLEs
                          join movieGenres in db.MOVIEGENRESTABLEs on movies.ID equals movieGenres.MOVIEID
                          join genres in db.GENRESTABLEs on movieGenres.GENRESID equals genres.ID

                          where (string.IsNullOrEmpty(searchText) || movies.TITLE.Contains(searchText) ||
                          movies.YEAROFRELEASE.ToString().Contains(searchText) ||
                          genres.GENRES.Contains(searchText))

                          select new SearchResponseModel()
                          {
                              Id = movies.ID,
                              Title = movies.TITLE,
                              Genres = genres.GENRES,
                              RunningTime = movies.RUNNINGTIME,
                              YearOfRelease = movies.YEAROFRELEASE
                          });

            if (result.Any())
            {
                response = new List<SearchResponseModel>();
            }
            else
                return response;

            List<SearchResponseModel> lstMovieRating = GetMoviesRating();
            List<SearchResponseModel> lstMovieGenres = GetGenres();

            foreach (long id in result.Select(x => x.Id).Distinct())
            {

                if (response != null)
                {
                    if (response.Where(x => x.Id.Equals(id)).Any()) continue;
                }

                SearchResponseModel model = result.Where(x => x.Id.Equals(id)).FirstOrDefault<SearchResponseModel>();

                if (lstMovieRating != null)
                {
                    if (lstMovieRating.Where(x => x.Id.Equals(model.Id)).Any())
                        model.AverageRating = lstMovieRating.Where(x => x.Id.Equals(model.Id)).FirstOrDefault<SearchResponseModel>().AverageRating;
                }

                if (lstMovieGenres != null)
                {
                    if (lstMovieGenres.Where(x => x.Id.Equals(model.Id)).Any())
                        model.Genres = lstMovieGenres.Where(x => x.Id.Equals(model.Id)).FirstOrDefault<SearchResponseModel>().Genres;
                }

                response.Add(model);
            }

            return response;
        }

        public static List<SearchResponseModel> GetTop5MoviesByUserRating(long userId)
        {
            ComcastEntities db = new ComcastEntities();
            List<SearchResponseModel> response = null;

            var result = (from movies in db.MOVIESTABLEs
                          join ra in db.RATINGTABLEs on movies.ID equals ra.MOVIEID
                          join movieGenres in db.MOVIEGENRESTABLEs on movies.ID equals movieGenres.MOVIEID
                          join genres in db.GENRESTABLEs on movieGenres.GENRESID equals genres.ID

                          where ra.CUSTOMERID.Equals(userId)

                          select new SearchResponseModel()
                          {
                              Id = movies.ID,
                              Title = movies.TITLE,
                              Genres = genres.GENRES,
                              RunningTime = movies.RUNNINGTIME,
                              YearOfRelease = movies.YEAROFRELEASE
                          });

            if (result.Any())
            {
                response = new List<SearchResponseModel>();
            }
            else
                return response;

            List<SearchResponseModel> lstMovieRating = GetMoviesRatingByUserId(userId);
            List<SearchResponseModel> lstMovieGenres = GetGenres();

            foreach (long id in result.Select(x => x.Id).Distinct())
            {

                if (response != null)
                {
                    if (response.Where(x => x.Id.Equals(id)).Any()) continue;
                }

                SearchResponseModel model = result.Where(x => x.Id.Equals(id)).FirstOrDefault<SearchResponseModel>();

                if (lstMovieRating != null)
                {
                    if (lstMovieRating.Where(x => x.Id.Equals(model.Id)).Any())
                        model.AverageRating = lstMovieRating.Where(x => x.Id.Equals(model.Id)).FirstOrDefault<SearchResponseModel>().AverageRating;
                }

                if (lstMovieGenres != null)
                {
                    if (lstMovieGenres.Where(x => x.Id.Equals(model.Id)).Any())
                        model.Genres = lstMovieGenres.Where(x => x.Id.Equals(model.Id)).FirstOrDefault<SearchResponseModel>().Genres;
                }

                response.Add(model);
            }

            return response;
        }

        private static List<SearchResponseModel> GetMoviesRating()
        {
            ComcastEntities db = new ComcastEntities();
            List<SearchResponseModel> movieRating = null;

            var result = (from movie in db.MOVIESTABLEs
                          join Rating in db.RATINGTABLEs on movie.ID equals Rating.MOVIEID
                          select new SearchResponseModel()
                          {
                              Id = movie.ID,
                              AverageRating = Rating.RATING
                          });

            if (result.Any())
            {
                movieRating = new List<SearchResponseModel>();
            }

            foreach (long id in result.Select(x => x.Id).Distinct())
            {
                if (movieRating != null && movieRating.Where(x => x.Id.Equals(id)).Any())
                    continue;

                var totalRows = result.Where(x => x.Id.Equals(id)).ToList().Count;

                var Rating = (result.Where(x => x.Id.Equals(id)).Sum(x => x.AverageRating) / totalRows);

                SearchResponseModel model = result.Where(x => x.Id.Equals(id)).FirstOrDefault<SearchResponseModel>();
                model.AverageRating = Math.Round(Rating * 2, MidpointRounding.AwayFromZero) / 2;

                movieRating.Add(model);
            }

            return movieRating;
        }

        private static List<SearchResponseModel> GetGenres()
        {
            ComcastEntities db = new ComcastEntities();
            List<SearchResponseModel> movieGenres = null;

            var result = (from movie in db.MOVIESTABLEs
                          join mGenres in db.MOVIEGENRESTABLEs on movie.ID equals mGenres.MOVIEID
                          join genres in db.GENRESTABLEs on mGenres.GENRESID equals genres.ID
                          select new SearchResponseModel()
                          {
                              Id = movie.ID,
                              Genres = genres.GENRES
                          });


            if (result.Any())
            {
                movieGenres = new List<SearchResponseModel>();
            }

            foreach (long id in result.Select(x => x.Id).Distinct())
            {
                if (movieGenres != null && movieGenres.Where(x => x.Id.Equals(id)).Any())
                    continue;

                var genres = string.Join(",", result.Where(x => x.Id.Equals(id)).Select(x => x.Genres).ToList());

                SearchResponseModel model = result.Where(x => x.Id.Equals(id)).FirstOrDefault<SearchResponseModel>();
                model.Genres = genres;

                movieGenres.Add(model);
            }

            return movieGenres;
        }

        private static List<SearchResponseModel> GetMoviesRatingByUserId(long userId)
        {
            ComcastEntities db = new ComcastEntities();
            List<SearchResponseModel> movieRating = null;

            var result = (from movie in db.MOVIESTABLEs
                          join Rating in db.RATINGTABLEs on movie.ID equals Rating.MOVIEID
                          where Rating.CUSTOMERID.Equals(userId)
                          select new SearchResponseModel()
                          {
                              Id = movie.ID,
                              AverageRating = Rating.RATING
                          });

            if (result.Any())
            {
                movieRating = new List<SearchResponseModel>();
            }

            foreach (long id in result.Select(x => x.Id).Distinct())
            {
                if (movieRating != null && movieRating.Where(x => x.Id.Equals(id)).Any())
                    continue;

                var totalRows = result.Where(x => x.Id.Equals(id)).ToList().Count;

                var Rating = (result.Where(x => x.Id.Equals(id)).Sum(x => x.AverageRating) / totalRows);

                SearchResponseModel model = result.Where(x => x.Id.Equals(id)).FirstOrDefault<SearchResponseModel>();
                model.AverageRating = Math.Round(Rating * 2, MidpointRounding.AwayFromZero) / 2;

                movieRating.Add(model);
            }

            return movieRating;
        }

        public static void SaveMovieRating(MovieRatingModel model)
        {
            ComcastEntities db = new ComcastEntities();

            db.RATINGTABLEs.Add(new DataService.RATINGTABLE()
            {
                CUSTOMERID = model.CustomerId,
                MOVIEID = model.MovieId,
                RATING = model.Rating,
                CREATEDDATETIME = DateTime.Now
            });

            db.SaveChanges();
        }

        public static bool CheckUserExists(long userId)
        {
            ComcastEntities db = new ComcastEntities();
            return db.CUSTOMERTABLEs.Where(x => x.ID.Equals(userId)).Any() ? true : false;
        }
    }
}
