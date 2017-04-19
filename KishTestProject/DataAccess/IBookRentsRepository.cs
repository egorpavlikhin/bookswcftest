using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KishTestProject.DataAccess
{
    public interface IBookRentsRepository : IRepository<BookRent, int>
    {
        bool RentABook(Book book, string borrower, DateTime borrowed);
        void ReturnABook(BookRent bookRent, DateTime returned);
        BookRent Find(Book book, string borrower);
    }
}
