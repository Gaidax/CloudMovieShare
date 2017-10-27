using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Datastore.V1;
using System.ComponentModel.DataAnnotations;

namespace Assignment3VasylMilchevskyi.Models
{
    public class Comment
    {
        [Key]
        public long id { get; set; }
        [Display(Name = "Comment: ")]
        [DataType(DataType.MultilineText)]
        public string comment { get; set; }
        public DateTime? created { get; set; }
        public long movie { get; set; }
        public string createdById { get; set; }

    }
}
