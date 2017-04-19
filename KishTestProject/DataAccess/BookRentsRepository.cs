using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KishTestProject.DataAccess
{
    public class BookRentsRepository : RepositoryBase<BookRent, int>, IBookRentsRepository
    {
        public BookRentsRepository(KishTestContext dataContext)
            : base(dataContext)
        {

        }

        public BookRent Find(Book book, string borrower)
        {
            return this.Get(x => x.Borrower == borrower && x.Book == book);
        }

        public bool RentABook(Book book, string borrower, DateTime borrowed)
        {
            if (book.CopiesLeft == 0 || book.Archived)
                return false;

            book.CopiesLeft--;
            this.Add(new BookRent { Book = book, Borrowed = borrowed, Borrower = borrower });

            return true;
        }

        public void ReturnABook(BookRent bookRent, DateTime returned)
        {
            bookRent.Book.CopiesLeft++;
            bookRent.Returned = returned;
        }
    }
}