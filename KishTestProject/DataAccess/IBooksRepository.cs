using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KishTestProject.DataAccess
{
    public interface IBooksRepository : IRepository<Book, string>
    {
        List<Book> FindByAuthors(List<string> author);
        Book FindByISBN(string isbn);
        bool Archive(string isbn);
    }
}
