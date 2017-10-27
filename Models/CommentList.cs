using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Assignment3VasylMilchevskyi.Models
{
    public class CommentList
    {
        public IEnumerable<Comment> comment;
        public string NextPageToken;
    }
}
