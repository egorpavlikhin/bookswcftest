using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace KishTestProject.DataAccess
{
    public class KishTestContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<BookRent> BookRents { get; set; }
        public DbSet<Author> Authors { get; set; }

        public KishTestContext() : base("KishTestConnection")
        {
            if (Books.Count() == 0)
            {
                this.InitializeData();
            }
        }

        private void InitializeData()
        {
            var kalsi = new Author { FullName = "Johan Kalsi" };
            var day = new Author { FullName = "Vox Day" };
            var hume = new Author { FullName = "David Hume" };
            Authors.Add(kalsi);
            Authors.Add(day);
            Authors.Add(hume);

            Books.Add(new Book { CopiesTotal = 1, CopiesLeft = 1, ISBN = "B06XFQ24QC", Title = "Corrosion (The Corroding Empire Book 1)", Type = BookType.Book, Authors = new List<Author> { kalsi, day } });
            Books.Add(new Book { CopiesTotal = 1, CopiesLeft = 1, ISBN = "B06Y2D7842", Title = "An Enquiry Concerning Human Understanding", Type = BookType.Ebook, Authors = new List<Author> { hume } });

            this.SaveChanges();
        }
    }
}