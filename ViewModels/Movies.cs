using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment3VasylMilchevskyi.ViewModels.Movies
{
        public class Form
        {
            /// <summary>
            /// The movie to be displayed in the form.
            /// </summary>
            public Models.Movie Movie;
            /// <summary>
            /// The string displayed to the user.  Either "Edit" or "Create".
            /// </summary>
            public string Action;

            /// <summary>
            /// False when the user tried entering a bad field value.  For example, they entered
            /// "yesterday" for Date Published.
            /// </summary>
            public bool IsValid;

            /// <summary>
            ///  The target of submit form.  Fills asp-action="".
            /// </summary>
            public string FormAction;
        }
}
