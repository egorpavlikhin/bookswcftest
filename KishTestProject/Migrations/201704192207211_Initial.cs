namespace KishTestProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Authors",
                c => new
                    {
                        AuthorId = c.Int(nullable: false, identity: true),
                        FullName = c.String(),
                        DOB = c.DateTime(),
                    })
                .PrimaryKey(t => t.AuthorId);
            
            CreateTable(
                "dbo.Books",
                c => new
                    {
                        ISBN = c.String(nullable: false, maxLength: 128),
                        Title = c.String(),
                        Annotation = c.String(),
                        Type = c.Int(nullable: false),
                        CopiesTotal = c.Int(nullable: false),
                        CopiesLeft = c.Int(nullable: false),
                        Archived = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ISBN);
            
            CreateTable(
                "dbo.BookRents",
                c => new
                    {
                        BookLeaseId = c.Int(nullable: false, identity: true),
                        Borrowed = c.DateTime(nullable: false),
                        Returned = c.DateTime(),
                        Borrower = c.String(),
                        Book_ISBN = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.BookLeaseId)
                .ForeignKey("dbo.Books", t => t.Book_ISBN)
                .Index(t => t.Book_ISBN);
            
            CreateTable(
                "dbo.BookAuthors",
                c => new
                    {
                        Book_ISBN = c.String(nullable: false, maxLength: 128),
                        Author_AuthorId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Book_ISBN, t.Author_AuthorId })
                .ForeignKey("dbo.Books", t => t.Book_ISBN, cascadeDelete: true)
                .ForeignKey("dbo.Authors", t => t.Author_AuthorId, cascadeDelete: true)
                .Index(t => t.Book_ISBN)
                .Index(t => t.Author_AuthorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BookRents", "Book_ISBN", "dbo.Books");
            DropForeignKey("dbo.BookAuthors", "Author_AuthorId", "dbo.Authors");
            DropForeignKey("dbo.BookAuthors", "Book_ISBN", "dbo.Books");
            DropIndex("dbo.BookAuthors", new[] { "Author_AuthorId" });
            DropIndex("dbo.BookAuthors", new[] { "Book_ISBN" });
            DropIndex("dbo.BookRents", new[] { "Book_ISBN" });
            DropTable("dbo.BookAuthors");
            DropTable("dbo.BookRents");
            DropTable("dbo.Books");
            DropTable("dbo.Authors");
        }
    }
}
