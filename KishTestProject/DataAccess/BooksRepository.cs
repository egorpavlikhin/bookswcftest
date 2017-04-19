using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KishTestProject.DataAccess
{
    public class BooksRepository : RepositoryBase<Book, string>, IBooksRepository
    {
        public BooksRepository(KishTestContext dataContext)
            : base(dataContext)
        {

        }

        public bool Archive(string isbn)
        {
            var book = this.FindByISBN(isbn);
            if (book == null)
                return false;

            book.Archived = true;

            return true;
        }

        public List<Book> FindByAuthors(List<string> author)
        {
            return 
                this.GetMany(x => x.Authors.Any(y => author.Contains(y.FullName)))
                .ToList();
        }

        public Book FindByISBN(string isbn)
        {
            return this.GetById(isbn);
        }
    }
}