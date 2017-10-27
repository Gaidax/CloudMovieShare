using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Datastore.V1;
using Assignment3VasylMilchevskyi.Models;
using Google.Protobuf;
using System.Diagnostics;

namespace Assignment3VasylMilchevskyi.Data
{
    public static class StorageAccessRatingExtensionMethods
    {
        /// <summary>
        /// Make a datastore key given a movie's id.
        /// </summary>
        /// <param name="id">A movie's id.</param>
        /// <returns>A movie key.</returns>

        /// <summary>
        /// Create a datastore entity with the same values as a movie.
        /// </summary>
        /// <param name="Movie">The book to store in datastore.</param>
        /// <returns>A datastore entity.</returns>
        /// [START toentity]
        public static Entity ToEntity(this Rating rating) => new Entity()
        {
            Key = rating.id.ToKey("Rating"),
            ["rating"] = rating.rating,
            ["movie"] = rating.movie,
            ["createdById"] = rating.createdById
        };

        public static Rating ToRating(this Entity entity) => new Rating()
        {
            id = entity.Key.Path.First().Id,
            rating = (double)entity["rating"],
            movie = (long)entity["movie"],
            createdById = (string)entity["createdById"]
        };
    }
    public class StorageAccessRating
    {
        private readonly ICommonDatastore _store;
        public StorageAccessRating(ICommonDatastore store)
        {
            _store = store;
        }
        public void DeleteRating(long id)
        {
            _store.DeleteEntity(id.ToKey("Rating"));
        }

        internal Rating GetRating(long id)
        {
            return _store.GetEntity(id.ToKey("Rating"))?.ToRating();
        }

        public void AddRating(Rating rating)
        {
            _store.Create(rating);
        }

        public void UpdateRating(Rating rating)
        {
            Rating newRating = GetRatingByUser(rating.
                    createdById, rating.movie);
            newRating.rating = rating.rating;
            _store.UpdateEntity(newRating.ToEntity());
        }

        public Rating GetRatingByUser(string createdById, long movieId)
        {
            var query = new Query("Rating")
            {
                Filter = Filter.And(Filter.Equal("movie", movieId),
                Filter.And(Filter.Equal("createdById", createdById)))
            };
            var results = _store.RunQuery(query);
            try
            {
                return results.Entities.Select(entity => entity.ToRating()).First();
            } catch
            {
                return null;
            }
        }

        public bool ThisUserRated(string createdById, long movieId)
        {
            if (GetRatingByUser(createdById, movieId) != null)
                return true;
            return false;
        }

        public void DeleteRatings()
        {
            var query = new Query("Rating");
            var results = _store.RunQuery(query);
            foreach(Rating rate in results.Entities.Select(entity => entity.ToRating())) {
                DeleteRating(rate.id);
            }
        }

        internal void DeleteRatings(long movieId)
        {
            foreach (Rating rate in ListRatings(movieId))
            {
                DeleteRating(rate.id);
            }
        }

        public double GetMovieRating(long movieId)
        {
            IEnumerable<Rating> ratings = ListRatings(movieId);
            if (ratings.Count() == 0)
                return 0;
            double finalRating = 0.0;
            foreach(Rating rat in ratings)
            {
                finalRating = (finalRating + rat.rating);
            }

            return finalRating / ratings.Count();
        }

        private IEnumerable<Rating> ListRatings(long movieId)
        {
            var query = new Query("Rating") {
                Filter = Filter.And(Filter.Equal("movie", movieId))};
            var results = _store.RunQuery(query);
            return results.Entities.Select(entity => entity.ToRating());
        }
    }
}
