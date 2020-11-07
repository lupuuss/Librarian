using Librarian.Model.Data;
using Librarian.Model.Data.Events;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Librarian.Logic
{
    public interface IDataService
    {
        void CreateCustomer(string name, string lastName, Address address);
        void LendBook(Customer customer, BookCopy bookCopy);
        void ReturnBook(Customer customer, BookCopy bookCopy);
        void ReturnDamagedBook(Customer customer, BookCopy bookCopy);
        IEnumerable<Customer> GetAllCustomers(string customerNameQuery = null);
        IEnumerable<Event> GetCustomerHistory(Customer customer);
        IEnumerable<Event> GetEvents(DateTime? fromDate = null, DateTime? toDate = null);
        Dictionary<Book, int> GetBooks();
        IEnumerable<BookCopy> GetAllCopies(Isbn isbn);
        void AddBook(Isbn isbn, string name, string author);
        void AddBookCopy(Book book, BookCopy.States state, double price);  
    }
}
