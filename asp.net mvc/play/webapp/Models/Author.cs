using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace webapp.Models
{
    public class Author
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string Biography { get; set; }

        public virtual ICollection<Book> Books { get; set; }
    }
}