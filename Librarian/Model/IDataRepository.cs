using Librarian.Model.Data;
using Librarian.Model.Data.Events;
using System.Collections.Generic;

namespace Librarian.Model
{
    public interface IDataRepository
    {
        void AddBook(Book position);
        Book GetBook(Isbn isbn);
        void DeleteBook(Isbn isbn);
        IEnumerable<Book> GetAllBooks();

        void AddCustomer(Customer customer);
        Customer GetCustomer(int id);
        void DeleteCustomer(Customer customer);
        IEnumerable<Customer> GetAllCustomers();

        void AddBookCopy(BookCopy bookCopy);
        BookCopy GetBookCopy(int id);
        void DeleteBookCopy(BookCopy bookCopy);
        IEnumerable<BookCopy> GetAllBookCopies();
        void AddEvent(Event eve);
        IEnumerable<Event> GetAllEvents();

    }
}
