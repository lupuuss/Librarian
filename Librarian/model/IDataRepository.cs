using Librarian.model.data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Librarian.model
{
    interface IDataRepository 
    {
        void AddBook(Book position);
        Book GetBook(Isbn isbn);
        void UpdateBook(Isbn isbn, Book position);
        void DeleteBook(Book position);
        IEnumerable<Book> GetAllBooks();
       

        void AddCustomer(Customer customer);
        Customer GetCustomer(int id);
        void DeleteCustomer(Customer customer);
        void UpdateCustomer(int id, Customer customer);
        IEnumerable<Customer> GetAllCustomers();

        void AddBookCopy(BookCopy bookCopy); 
        BookCopy GetBookCopy(int id);
        void UpdateBookCopy(int id, BookCopy bookCopy);
        void DeleteBookCopy(BookCopy bookCopy);
        IEnumerable<BookCopy> GetAllBookCopies();
        
       

        



    }
}
