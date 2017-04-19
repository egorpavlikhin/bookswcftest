using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KishTestProject.DataAccess
{
    public enum BookType
    {
        Book = 1,
        Ebook,
        Magazine,
        Newspaper
    }

    public class Book
    {
        public Book()
        {
            this.Authors = new HashSet<Author>();
        }

        public string Title { get; set; }

        public string Annotation { get; set; }

        [Key]
        public string ISBN { get; set; }

        public BookType Type { get; set; }

        public int CopiesTotal { get; set; }

        public int CopiesLeft { get; set; }

        public virtual ICollection<Author> Authors { get; set; }

        public bool Archived { get; set; }
    }
}