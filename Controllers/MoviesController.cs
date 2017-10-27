using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Assignment3VasylMilchevskyi.Data;
using Assignment3VasylMilchevskyi.Models;
using Assignment3VasylMilchevskyi.Services;
using System.Diagnostics;

namespace Assignment3VasylMilchevskyi.Controllers
{
    public class MoviesController : Controller
    {
        private const int _pageSize = 10;
        private readonly StorageAccessMovie _movieStore;
        private readonly StorageAccessComment _commentStore;
        private readonly StorageAccessRating _ratingStore;
        private readonly ImageUploader _imageUploader;

        public MoviesController(StorageAccessMovie movieStore, StorageAccessComment commentStore,
            StorageAccessRating ratingStore, ImageUploader imageUploader)
        {
            _movieStore = movieStore;
            _commentStore = commentStore;
            _ratingStore = ratingStore;
            _imageUploader = imageUploader;
        }
        // GET: Movie
        public ActionResult Index(string nextPageToken)
        {
            return View(new ViewModels.Movies.Index()
            {
                MovieList = _movieStore.ListMovies(_pageSize, nextPageToken)
            });
        }

        // GET: Movies/AddMovie
        public ActionResult AddMovie()
        {
            return ViewForm("AddMovie", "AddMovie");
        }
        //POST: Movies/AddMovie
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddMovie(Movie movie, IFormFile image, IFormFile movieFile)
        {
            if (ModelState.IsValid)
            {
                movie.createdById = User.Identity.Name;
                long movieId = _movieStore.AddMovie(movie);
                _ratingStore.AddRating(new Rating
                {
                    createdById = User.Identity.Name,
                    movie = movieId,
                    rating = 1.0
                });
                
                if (image != null)
                {
                        var imageUrl = await _imageUploader.UploadFile(image, movie.id, Variables.imageMovieBucket);
                        movie.imageUrl = imageUrl;
                        _movieStore.UpdateMovie(movie);
                    
                }
                if (movieFile != null)
                {
                    var movieUrl = await _imageUploader.UploadFile(movieFile, movie.id, Variables.movieBucket);
                    movie.movieUrl = movieUrl;
                    _movieStore.UpdateMovie(movie);
                }

                return RedirectToAction("Details", new { id = movie.id });
            }
            return ViewForm("AddMovie", "AddMovie", movie);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddComment(string commentStr, string movieId)
        {
            Comment comment = new Comment();
            comment.comment = commentStr;
            comment.movie = long.Parse(movieId);
            comment.createdById = User.Identity.Name;
            comment.created = DateTime.Now;
            _commentStore.AddComment(comment);
            return RedirectToAction("Details", new { id = comment.movie });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteComment(long id)
        {
            long movieId = _commentStore.GetComment(id).movie;
            _commentStore.DeleteComment(id);
            return RedirectToAction("Details", new { id = movieId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RateMovie(string movieId, string ratingNum)
        {
            Rating rate = new Rating();
            rate.createdById = User.Identity.Name;
            rate.movie = long.Parse(movieId);
            rate.rating = float.Parse(ratingNum);
            if (_ratingStore.ThisUserRated(rate.createdById, rate.movie))
            {
                //Debug.WriteLine("!===========" + rate.rating);
                _ratingStore.UpdateRating(rate);
            }
            else {
                //Debug.WriteLine("!===========" + rate.rating);
                _ratingStore.AddRating(rate);
        }
            
            return RedirectToAction("Details", new { id = movieId });
        }


        // GET: Movie/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Movie/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // POST: Movie/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteMovie(long id)
        {
            try
            {
                _ratingStore.DeleteRatings(id);
                _commentStore.DeleteComments(id);
                _movieStore.DeleteMovie(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        public ActionResult Details(long? id, string nextPageToken)
        {
            if (id == null || id == 0)
                return Redirect("Error");

            Movie mov = _movieStore.GetMovie((long)id);
            if (mov == null)
                return Redirect("Error");

            double rat = _ratingStore.GetMovieRating((long)id);

            return View(new ViewModels.Movies.Details()
            {
                Comments = _commentStore.ListMovieComments((long)id, _pageSize, nextPageToken),
                Movie = mov,
                Rating = rat
            });
        }
        private ActionResult ViewForm(string action, string formAction, Movie movie = null)
        {
            var form = new ViewModels.Movies.Form()
            {
                Action = action,
                Movie = movie ?? new Movie(),
                IsValid = ModelState.IsValid,
                FormAction = formAction
            };
            return View("/Views/Movies/Create.cshtml", form);
        }
    }
}