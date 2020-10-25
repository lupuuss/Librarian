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
        IEnumerable<Book> GetAllBooks();
        void UpdateBook(Isbn isbn, Book position);
        void DeleteBook(Book position);
        void AddCustomer(Customer customer);
        Customer GetCustomer(Guid guid);
        IEnumerable<Customer> GetAllCustomers();
        void UpdateCustomer(Guid guid, Customer customer);
        void DeleteCustomer(Customer customer); 



    }
}
