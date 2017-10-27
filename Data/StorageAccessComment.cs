using System;
using System.Linq;
using Google.Cloud.Datastore.V1;
using Assignment3VasylMilchevskyi.Models;
using Google.Protobuf;

namespace Assignment3VasylMilchevskyi.Data
{
    public static class StorageAccessCommentExtensionMethods
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
        public static Entity ToEntity(this Comment comment) => new Entity()
        {
            Key = comment.id.ToKey("Comment"),
            ["commentText"] = comment.comment,
            ["movie"] = comment.movie,
            ["created"] = comment.created?.ToUniversalTime(),
            ["createdById"] = comment.createdById
        };

        public static Comment ToComment(this Entity entity) => new Comment()
        {
            id = entity.Key.Path.First().Id,
            comment = (string)entity["commentText"],
            movie = (long)entity["movie"],
            created = (DateTime?)entity["created"],
            createdById = (string)entity["createdById"]
        };
    }
    public class StorageAccessComment
    {
        private readonly ICommonDatastore _store;
        public StorageAccessComment(ICommonDatastore store)
        {
            _store = store;
        }
        public void DeleteComment(long id)
        {
            _store.DeleteEntity(id.ToKey("Comment"));
        }

        internal Comment GetComment(long id)
        {
            return _store.GetEntity(id.ToKey("Comment"))?.ToComment();
        }

        public void AddComment(Comment comment)
        {
            _store.Create(comment);
        }

        public void DeleteComments()
        {
            var query = new Query("Comment");
            var results = _store.RunQuery(query);
            foreach (Comment comm in new CommentList()
            {
                comment = results.Entities.Select(entity => entity.ToComment()),
                NextPageToken = results.Entities.Count == query.Limit ?
                    results.EndCursor.ToBase64() : null
            }.comment)
            {
                DeleteComment(comm.id);
            }
        }

        public void DeleteComments(long movieId)
        {
            var query = new Query("Comment")
            {
                Filter = Filter.And(Filter.Equal("movie", movieId))
            };
            var results = _store.RunQuery(query);
            foreach (Comment comm in new CommentList()
            {
                comment = results.Entities.Select(entity => entity.ToComment()),
                NextPageToken = results.Entities.Count == query.Limit ?
                    results.EndCursor.ToBase64() : null
            }.comment)
            {
                DeleteComment(comm.id);
            }
        }

        public CommentList ListMovieComments(long movieId, int pageSize, string nextPageToken)
        {
            var query = new Query("Comment")
            {
                Filter = Filter.And(Filter.Equal("movie", movieId)),
                //Order = { { "created", PropertyOrder.Types.Direction.Descending } },
                Limit = pageSize
            };
            if (!string.IsNullOrWhiteSpace(nextPageToken))
                query.StartCursor = ByteString.FromBase64(nextPageToken);
            var results = _store.RunQuery(query);
            return new CommentList()
            {
                comment = results.Entities.Select(entity => entity.ToComment()),
                NextPageToken = results.Entities.Count == query.Limit ?
                    results.EndCursor.ToBase64() : null
            };
        }
    }
}
