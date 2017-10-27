using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Datastore.V1;
using System.ComponentModel.DataAnnotations;

namespace Assignment3VasylMilchevskyi.Models
{
    public class Rating
    {
        [Key]
        public long id { get; set; }
        public double rating { get; set; }
        public long movie { get; set; }
        public string createdById { get; set; }

    }
}
