using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

using KishTestProject.DataAccess;
using System.Security.Permissions;

namespace KishTestProject
{
    public class BookService : IBookService
    {
        private IBooksRepository booksRepository;
        private IBookRentsRepository bookRentsRepository;

        public BookService(IBooksRepository booksRepository, IBookRentsRepository bookRentsRepository)
        {
            this.booksRepository = booksRepository ?? throw new ArgumentNullException("booksRepository");
            this.bookRentsRepository = bookRentsRepository ?? throw new ArgumentNullException("bookRentsRepository");
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Senior")]
        public bool Archive(string isbn)
        {
            if (string.IsNullOrWhiteSpace(isbn))
                throw new ArgumentNullException("isbn");

            if (this.booksRepository.Archive(isbn))
            {
                this.booksRepository.Commit();
                return true;
            }

            return false;
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Junior")]
        [PrincipalPermission(SecurityAction.Demand, Role = "Senior")]
        public bool Borrow(string isbn, string borrower)
        {
            if (string.IsNullOrWhiteSpace(isbn))
                throw new ArgumentNullException("isbn");
            if (string.IsNullOrWhiteSpace(borrower))
                throw new ArgumentNullException("borrower");

            var book = this.booksRepository.FindByISBN(isbn);
            if (book == null)
                return false; // TODO: return something more informative

            // TODO: can the same person rent a book twice?
            if (this.bookRentsRepository.RentABook(book, borrower, DateTime.Now))
            {
                this.bookRentsRepository.Commit();
                return true;
            }

            return false;
        }

        public bool CanBeBorrowed(string isbn)
        {
            if (string.IsNullOrWhiteSpace(isbn))
                throw new ArgumentNullException("isbn");

            var book = this.booksRepository.FindByISBN(isbn);
            if (book == null)
                return false; // TODO: return something more informative

            return book.CopiesLeft > 0 && !book.Archived;
        }

        public List<Book> FindByAuthors(List<string> author)
        {
            if (author == null || author.Count == 0)
                throw new ArgumentNullException("author");

            // TODO: use Automapper
            return this.booksRepository.FindByAuthors(author)
                .Select<DataAccess.Book, Book>(x =>
                {
                    return new Book { Annotation = x.Annotation, Authors = x.Authors.Select(a => new Author { FullName = a.FullName, DOB = a.DOB }).ToList(), ISBN = x.ISBN, Title = x.Title, Type = x.Type };
                })
                .ToList();
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Intern")]
        [PrincipalPermission(SecurityAction.Demand, Role = "Junior")]
        [PrincipalPermission(SecurityAction.Demand, Role = "Senior")]
        // TODO: return something bettter than just bool
        public bool Return(string isbn, string borrower)
        {
            if (string.IsNullOrWhiteSpace(isbn))
                throw new ArgumentNullException("isbn");
            if (string.IsNullOrWhiteSpace(borrower))
                throw new ArgumentNullException("borrower");

            var book = this.booksRepository.FindByISBN(isbn);
            if (book == null)
                return false;

            var bookRent = this.bookRentsRepository.Find(book, borrower);
            if (bookRent == null)
                return false;

            this.bookRentsRepository.ReturnABook(bookRent, DateTime.Now);
            this.bookRentsRepository.Commit();
            return true;
        }
    }
}
