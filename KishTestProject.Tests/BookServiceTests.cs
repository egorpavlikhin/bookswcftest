using KishTestProject.DataAccess;
using Moq;
using Ploeh.AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace KishTestProject.Tests
{
    public class BookServiceTests
    {
        private Fixture fixture = new Fixture();

        public BookServiceTests()
        {            
        }

        public void SetIdentity(string role)
        {
            var identity = new GenericIdentity("Unitest");
            System.Threading.Thread.CurrentPrincipal = new GenericPrincipal(identity, new[] { role });
        }

        [Fact]
        public void CanArchive()
        {
            SetIdentity("Senior");
            string isbn = fixture.Create<string>();

            var bookRepository = new Mock<IBooksRepository>();
            bookRepository.Setup(x => x.Archive(isbn)).Returns(true);

            var service = new BookService(bookRepository.Object, new Mock<IBookRentsRepository>().Object);
            Assert.True(service.Archive(isbn));

            bookRepository.Verify(x => x.Archive(isbn));
            bookRepository.Verify(x => x.Commit());
        }

        [Fact]
        public void OnlySeniorCanArchive()
        {
            SetIdentity("Junior");
            string isbn = fixture.Create<string>();

            var bookRepository = new Mock<IBooksRepository>();

            var service = new BookService(bookRepository.Object, new Mock<IBookRentsRepository>().Object);
            Assert.Throws<SecurityException>(() => service.Archive(isbn));

            bookRepository.Verify(x => x.Archive(isbn), Times.Never);
            bookRepository.Verify(x => x.Commit(), Times.Never);
        }

        [Fact]
        public void ArchiveThrowsWhenISBNIsNull()
        {
            SetIdentity("Senior");

            var bookRepository = new Mock<IBooksRepository>();
            bookRepository.Setup(x => x.Archive(null)).Returns(true);

            var service = new BookService(bookRepository.Object, new Mock<IBookRentsRepository>().Object);
            Assert.Throws<ArgumentNullException>(() => service.Archive(null));

            bookRepository.Verify(x => x.Commit(), Times.Never);
        }

        [Fact]
        public void ArchiveReturnsIfNoBookFound()
        {
            SetIdentity("Senior");
            string isbn = fixture.Create<string>();

            var bookRepository = new Mock<IBooksRepository>();
            bookRepository.Setup(x => x.Archive(isbn)).Returns(false);

            var service = new BookService(bookRepository.Object, new Mock<IBookRentsRepository>().Object);
            Assert.False(service.Archive(isbn));

            bookRepository.Verify(x => x.Archive(isbn));
            bookRepository.Verify(x => x.Commit(), Times.Never);
        }

        [Fact]
        public void CanBorrow()
        {
            SetIdentity("Junior");
            string isbn = fixture.Create<string>();
            string borrower = fixture.Create<string>();

            var book = new DataAccess.Book { CopiesLeft = 1 };
            var bookRepository = new Mock<IBooksRepository>();
            bookRepository.Setup(x => x.FindByISBN(isbn)).Returns(book);
            var bookRentRepository = new Mock<IBookRentsRepository>();
            bookRentRepository.Setup(x => x.RentABook(book, borrower, It.IsAny<DateTime>())).Returns(true);

            var service = new BookService(bookRepository.Object, bookRentRepository.Object);
            Assert.True(service.Borrow(isbn, borrower));

            bookRentRepository.Verify(x => x.RentABook(book, borrower, It.IsAny<DateTime>()));
            bookRentRepository.Verify(x => x.Commit());
        }

        [Fact]
        public void OnlySeniorAndJuniorCanBorrow()
        {
            SetIdentity("Intern");
            
            var service = new BookService(new Mock<IBooksRepository>().Object, new Mock<IBookRentsRepository>().Object);
            Assert.Throws<SecurityException>(() => service.Borrow("", ""));
        }

        [Fact]
        public void BorrowThrowsWhenISBNIsNull()
        {
            SetIdentity("Junior");

            var bookRepository = new Mock<IBooksRepository>();
            var bookRentRepository = new Mock<IBookRentsRepository>();
            bookRepository.Setup(x => x.Archive(null)).Returns(true);

            var service = new BookService(bookRepository.Object, bookRentRepository.Object);
            Assert.Throws<ArgumentNullException>(() => service.Borrow(null, null));

            bookRepository.Verify(x => x.Commit(), Times.Never);
        }

        [Fact]
        public void BorrowThrowsWhenBorrowerIsNull()
        {
            SetIdentity("Junior");

            string isbn = fixture.Create<string>();

            var bookRepository = new Mock<IBooksRepository>();
            var bookRentRepository = new Mock<IBookRentsRepository>();
            bookRepository.Setup(x => x.Archive(null)).Returns(true);

            var service = new BookService(bookRepository.Object, bookRentRepository.Object);
            Assert.Throws<ArgumentNullException>(() => service.Borrow(isbn, null));

            bookRepository.Verify(x => x.Commit(), Times.Never);
        }

        [Fact]
        public void BorrowReturnsFalseIfNoBookFound()
        {
            SetIdentity("Junior");
            string isbn = fixture.Create<string>();
            string borrower = fixture.Create<string>();

            var bookRepository = new Mock<IBooksRepository>();
            var bookRentRepository = new Mock<IBookRentsRepository>();
            bookRepository.Setup(x => x.FindByISBN(isbn)).Returns<DataAccess.Book>(null);

            var service = new BookService(bookRepository.Object, bookRentRepository.Object);
            Assert.False(service.Borrow(isbn, borrower));

            bookRepository.Verify(x => x.FindByISBN(isbn));
            bookRentRepository.Verify(x => x.Commit(), Times.Never);
        }

        [Fact]
        public void BorrowReturnsFalseIfBookCannotBeBorrowed()
        {
            SetIdentity("Junior");
            string isbn = fixture.Create<string>();
            string borrower = fixture.Create<string>();

            var book = new DataAccess.Book { CopiesLeft = 0 };
            var bookRepository = new Mock<IBooksRepository>();
            bookRepository.Setup(x => x.FindByISBN(isbn)).Returns(book);
            var bookRentRepository = new Mock<IBookRentsRepository>();
            bookRentRepository.Setup(x => x.RentABook(book, borrower, It.IsAny<DateTime>())).Returns(false);

            var service = new BookService(bookRepository.Object, bookRentRepository.Object);
            Assert.False(service.Borrow(isbn, borrower));

            bookRentRepository.Verify(x => x.RentABook(book, borrower, It.IsAny<DateTime>()));
            bookRentRepository.Verify(x => x.Commit(), Times.Never);
        }

        [Fact]
        public void CanBeBorrowedReturnsCorrectly()
        {
            string isbn = fixture.Create<string>();

            var book = new DataAccess.Book { CopiesLeft = 1 };
            var bookRepository = new Mock<IBooksRepository>();
            bookRepository.Setup(x => x.FindByISBN(isbn)).Returns(book);
            var bookRentRepository = new Mock<IBookRentsRepository>();

            var service = new BookService(bookRepository.Object, bookRentRepository.Object);
            Assert.True(service.CanBeBorrowed(isbn));

            bookRepository.Verify(x => x.FindByISBN(isbn));
        }

        [Fact]
        public void CanBeBorrowedReturnsFalseBookArchived()
        {
            string isbn = fixture.Create<string>();

            var book = new DataAccess.Book { CopiesLeft = 1, Archived = true };
            var bookRepository = new Mock<IBooksRepository>();
            bookRepository.Setup(x => x.FindByISBN(isbn)).Returns(book);
            var bookRentRepository = new Mock<IBookRentsRepository>();

            var service = new BookService(bookRepository.Object, bookRentRepository.Object);
            Assert.False(service.CanBeBorrowed(isbn));

            bookRepository.Verify(x => x.FindByISBN(isbn));
        }

        [Fact]
        public void CanBeBorrowedThrowsISBNIsNull()
        {
            string isbn = null;
            
            var bookRepository = new Mock<IBooksRepository>();
            var bookRentRepository = new Mock<IBookRentsRepository>();

            var service = new BookService(bookRepository.Object, bookRentRepository.Object);
            Assert.Throws<ArgumentNullException>(() => service.CanBeBorrowed(isbn));
        }

        [Fact]
        public void CanBeBorrowedReturnsFalseNoCopies()
        {
            string isbn = fixture.Create<string>();

            var book = new DataAccess.Book { CopiesLeft = 0, Archived = false };
            var bookRepository = new Mock<IBooksRepository>();
            bookRepository.Setup(x => x.FindByISBN(isbn)).Returns(book);
            var bookRentRepository = new Mock<IBookRentsRepository>();

            var service = new BookService(bookRepository.Object, bookRentRepository.Object);
            Assert.False(service.CanBeBorrowed(isbn));

            bookRepository.Verify(x => x.FindByISBN(isbn));
        }

        [Fact]
        public void CanBeBorrowedReturnsFalseNoBook()
        {
            string isbn = fixture.Create<string>();
            
            var bookRepository = new Mock<IBooksRepository>();
            bookRepository.Setup(x => x.FindByISBN(isbn)).Returns<DataAccess.Book>(null);
            var bookRentRepository = new Mock<IBookRentsRepository>();

            var service = new BookService(bookRepository.Object, bookRentRepository.Object);
            Assert.False(service.CanBeBorrowed(isbn));

            bookRepository.Verify(x => x.FindByISBN(isbn));
        }

        [Fact]
        public void CanFindBookByAuthor()
        {
            var author = new DataAccess.Author { FullName = fixture.Create<string>() };

            var book = new DataAccess.Book { Title = fixture.Create<string>() };
            book.Authors.Add(author);
            var bookRepository = new Mock<IBooksRepository>();
            bookRepository.Setup(x => x.FindByAuthors(new List<string> { author.FullName })).Returns(new List<DataAccess.Book> { book });
            var bookRentRepository = new Mock<IBookRentsRepository>();

            var service = new BookService(bookRepository.Object, bookRentRepository.Object);
            Assert.Collection(service.FindByAuthors(new List<string> { author.FullName }),
                elem1 =>
                {
                    Assert.Equal(book.Title, elem1.Title);
                    Assert.Equal(author.FullName, elem1.Authors[0].FullName);
                });

            bookRepository.Verify(x => x.FindByAuthors(new List<string> { author.FullName }));
        }

        [Fact]
        public void FindAuthorsThrowsIfAuthorsNullOrEmpty()
        {
            var bookRepository = new Mock<IBooksRepository>();
            var bookRentRepository = new Mock<IBookRentsRepository>();

            var service = new BookService(bookRepository.Object, bookRentRepository.Object);
            Assert.Throws<ArgumentNullException>(() => service.FindByAuthors(null));
            Assert.Throws<ArgumentNullException>(() => service.FindByAuthors(new List<string>()));
        }

        [Fact]
        public void CanReturnABook()
        {
            SetIdentity("Junior");
            string isbn = fixture.Create<string>();
            string borrower = fixture.Create<string>();

            var book = new DataAccess.Book { CopiesLeft = 0, ISBN = isbn };
            var bookRent = new BookRent { Book = book, Borrower = borrower, Borrowed = DateTime.Now };
            var bookRepository = new Mock<IBooksRepository>();
            bookRepository.Setup(x => x.FindByISBN(isbn)).Returns(book);
            var bookRentRepository = new Mock<IBookRentsRepository>();
            bookRentRepository.Setup(x => x.Find(book, borrower)).Returns(bookRent);
            bookRentRepository.Setup(x => x.ReturnABook(bookRent, It.IsAny<DateTime>()));

            var service = new BookService(bookRepository.Object, bookRentRepository.Object);
            Assert.True(service.Return(isbn, borrower));

            bookRepository.Verify(x => x.FindByISBN(isbn));
            bookRentRepository.Verify(x => x.Find(book, borrower));
            bookRentRepository.Verify(x => x.ReturnABook(bookRent, It.IsAny<DateTime>()));
            bookRentRepository.Verify(x => x.Commit());
        }

        [Fact]
        public void ReturnReturnsFalseIfNoBookFound()
        {
            SetIdentity("Junior");
            string isbn = fixture.Create<string>();
            string borrower = fixture.Create<string>();

            var bookRepository = new Mock<IBooksRepository>();
            var bookRentRepository = new Mock<IBookRentsRepository>();
            bookRepository.Setup(x => x.FindByISBN(isbn)).Returns<DataAccess.Book>(null);

            var service = new BookService(bookRepository.Object, bookRentRepository.Object);
            Assert.False(service.Return(isbn, borrower));

            bookRepository.Verify(x => x.FindByISBN(isbn));
            bookRentRepository.Verify(x => x.Commit(), Times.Never);
        }

        [Fact]
        public void ReturnReturnsFalseIfBookRentNotFound()
        {
            SetIdentity("Junior");
            string isbn = fixture.Create<string>();
            string borrower = fixture.Create<string>();

            var book = new DataAccess.Book { CopiesLeft = 0, ISBN = isbn };
            var bookRepository = new Mock<IBooksRepository>();
            bookRepository.Setup(x => x.FindByISBN(isbn)).Returns(book);
            var bookRentRepository = new Mock<IBookRentsRepository>();
            bookRentRepository.Setup(x => x.Find(book, borrower)).Returns<BookRent>(null);

            var service = new BookService(bookRepository.Object, bookRentRepository.Object);
            Assert.False(service.Return(isbn, borrower));

            bookRepository.Verify(x => x.FindByISBN(isbn));
            bookRentRepository.Verify(x => x.Find(book, borrower));
            bookRentRepository.Verify(x => x.Commit(), Times.Never);
        }

        [Fact]
        public void ReturnThrowsIfISBNOrBorrowerNullOrEmpty()
        {
            SetIdentity("Junior");
            string isbn = fixture.Create<string>();
            string borrower = fixture.Create<string>();

            var bookRepository = new Mock<IBooksRepository>();
            var bookRentRepository = new Mock<IBookRentsRepository>();

            var service = new BookService(bookRepository.Object, bookRentRepository.Object);
            Assert.Throws<ArgumentNullException>(() => service.Return(null, borrower));
            Assert.Throws<ArgumentNullException>(() => service.Return(isbn, null));
            Assert.Throws<ArgumentNullException>(() => service.Return(null, null));
            Assert.Throws<ArgumentNullException>(() => service.Return("", ""));
            Assert.Throws<ArgumentNullException>(() => service.Return("", borrower));
            Assert.Throws<ArgumentNullException>(() => service.Return(isbn, ""));
        }

        [Fact]
        public void OnlyAppropriateRoleCanReturn()
        {
            SetIdentity("NotJunior");

            string isbn = fixture.Create<string>();
            string borrower = fixture.Create<string>();

            var bookRepository = new Mock<IBooksRepository>();
            var bookRentRepository = new Mock<IBookRentsRepository>();

            var service = new BookService(bookRepository.Object, bookRentRepository.Object);
            Assert.Throws<SecurityException>(() => service.Return(isbn, borrower));
        }
    }
}
