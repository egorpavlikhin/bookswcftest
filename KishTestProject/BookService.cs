using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace KishTestProject
{
    [ServiceContract]
    public interface IBookService
    {
        [OperationContract]
        List<Book> FindByAuthors(List<string> author);

        [OperationContract]
        bool CanBeBorrowed(string isbn);

        [OperationContract]
        bool Borrow(string isbn, string borrower);

        [OperationContract]
        bool Return(string isbn, string borrower);

        [OperationContract]
        bool Archive(string isbn);
    }

    [DataContract]
    public class Author
    {
        [DataMember]
        public string FullName { get; set; }

        [DataMember]
        public DateTime? DOB { get; set; }
    }

    [DataContract]
    public class Book
    {
        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Annotation { get; set; }
        
        [DataMember]
        public string ISBN { get; set; }

        [DataMember]
        public DataAccess.BookType Type { get; set; }

        [DataMember]
        public List<Author> Authors { get; set; }
    }
}
