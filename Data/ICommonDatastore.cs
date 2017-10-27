using Google.Cloud.Datastore.V1;
using System;
using System.Collections.Generic;

namespace Assignment3VasylMilchevskyi.Data
{
        public interface ICommonDatastore
        {
            void UpdateEntity(Entity ent);
            void DeleteEntity(Key id);
            long Create(Object obj);
            Entity GetEntity(Key id);
            DatastoreQueryResults RunQuery(Query query);
        }
    
}
