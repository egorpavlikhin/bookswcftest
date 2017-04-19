using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace KishTestProject.DataAccess
{
    public class BookRent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookLeaseId { get; set; }

        public Book Book { get; set; }

        public DateTime Borrowed { get; set; }

        public DateTime? Returned { get; set; }

        public string Borrower { get; set; }
    }
}