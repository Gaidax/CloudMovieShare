using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Datastore.V1;
using Google.Protobuf.WellKnownTypes;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Assignment3VasylMilchevskyi.Data;

namespace Assignment3VasylMilchevskyi.Models
{
    [Bind(include: "title, description, created, rating")]
    public class Movie
    {
        [Key]
        public long id { get; set; }
        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public String description { get; set; }
        [Display(Name = "Title")]
        public String title { get; set; }
        [Display(Name = "Date Created")]
        [DataType(DataType.Date)]
        public DateTime? created { get; set; }
        public String imageUrl { get; set; }
        public String movieUrl { get; set; }
        public string createdById { get; set; }
    }
}
