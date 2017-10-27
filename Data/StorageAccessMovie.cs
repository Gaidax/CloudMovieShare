using System;
using System.Linq;
using Google.Cloud.Datastore.V1;
using Assignment3VasylMilchevskyi.Models;
using Google.Protobuf;

namespace Assignment3VasylMilchevskyi.Data
{
    public static class StorageAccessMovieExtensionMethods
    {
 
        public static Entity ToEntity(this Movie movie) => new Entity()
        {
            Key = movie.id.ToKey("Movie"),
            ["title"] = movie.title,
            ["description"] = movie.description,
            ["created"] = movie.created?.ToUniversalTime(),
            ["imageUrl"] = movie.imageUrl,
            ["movieUrl"] = movie.movieUrl,
            ["createdById"] = movie.createdById
        };

        public static Movie ToMovie(this Entity entity) => new Movie()
        {
            id = entity.Key.Path.First().Id,
            title = (string)entity["title"],
            description = (string)entity["description"],
            created = (DateTime?)entity["created"],
            imageUrl = (string)entity["imageUrl"],
            movieUrl = (string)entity["movieUrl"],
            createdById = (string)entity["createdById"]
        };
    }
        public class StorageAccessMovie
        {
        private readonly ICommonDatastore _store;
        public StorageAccessMovie(ICommonDatastore store)
        {
            _store = store;
        }

        public MovieList ListMovies(int pageSize, string nextPageToken)
        {
            var query = new Query("Movie") { Limit = pageSize };
            if (!string.IsNullOrWhiteSpace(nextPageToken))
                query.StartCursor = ByteString.FromBase64(nextPageToken);
            var results = _store.RunQuery(query);
            return new MovieList()
            {
                movie = results.Entities.Select(entity => entity.ToMovie()),
                NextPageToken = results.Entities.Count == query.Limit ?
                    results.EndCursor.ToBase64() : null
            };
        }

        public void DeleteMovies()
        {
            var query = new Query("Movie");
            var results = _store.RunQuery(query);
            foreach(Movie mov in results.Entities.Select(entity => entity.ToMovie()))
            {
                DeleteMovie(mov.id);
            }
        }

        public long AddMovie(Movie movie)
        {
            return _store.Create(movie);
        }

        public void DeleteMovie(long id)
        {
            _store.DeleteEntity(id.ToKey("Movie"));
        }

        internal Movie GetMovie(long id)
        {
            return _store.GetEntity(id.ToKey("Movie"))?.ToMovie();
        }

        public void UpdateMovie(Movie newMovie)
        {
            _store.UpdateEntity(newMovie.ToEntity());
        }
    }
}
