using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Datastore.V1;
using Assignment3VasylMilchevskyi.Models;

namespace Assignment3VasylMilchevskyi.Data
{
    public static class CommonStorageAccessExtensionMethods
    {
        public static Key ToKey(this long id, string kind) =>
    new Key().WithElement(kind, id);

        /// <summary>
        /// Make an object id given a datastore key.
        /// </summary>
        /// <param name="key">A datastore key</param>
        /// <returns>An object id.</returns>
        public static long ToId(this Key key) => key.Path.First().Id;
    }

    internal class CommonStorageAccessDao : ICommonDatastore
    {
        private readonly DatastoreDb _db;

        public CommonStorageAccessDao()
        {
            _db = DatastoreDb.Create(Variables.projectId);
        }

        private Key CreateEntKey(String kind)
        {
            return _db.CreateKeyFactory(kind).CreateIncompleteKey();
        }

        public long Create(Movie movie)
        {
            var entity = movie.ToEntity();
            entity.Key = CreateEntKey("Movie");
            var keys = _db.Insert(new[] { entity });
            movie.id = keys.First().Path.First().Id;
            return movie.id;
        }

        public long Create(Comment comment)
        {
            var entity = comment.ToEntity();
            entity.Key = CreateEntKey("Comment");
            var keys = _db.Insert(new[] { entity });
            comment.id = keys.First().Path.First().Id;
            return comment.id;
        }

        public long Create(Rating rating)
        {
            var entity = rating.ToEntity();
            entity.Key = CreateEntKey("Rating");
            var keys = _db.Insert(new[] { entity });
            rating.id = keys.First().Path.First().Id;
            return rating.id;
        }

        public long Create(Object obj)
        {
            if (obj is Comment)
               return Create((Comment)obj);
            if (obj is Movie)
               return Create((Movie)obj);
            if (obj is Rating)
               return Create((Rating)obj);

            return 0;
        }
 
        public DatastoreQueryResults RunQuery(Query query)
        {
            return _db.RunQuery(query);
        }

        public void UpdateEntity(Entity ent)
        {
            _db.Update(ent);
        }

        public void DeleteEntity(Key id)
        {
            _db.Delete(id);
        }

        public Entity GetEntity(Key id)
        {
            return _db.Lookup(id);
        }
    }
}