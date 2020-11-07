using Librarian.Model.Data;
using Librarian.Model.Data.Events;
using System.Collections.Generic;

namespace Librarian.Model
{
    public interface IDataRepository
    {
        void AddBook(Book position);
        Book GetBook(Isbn isbn);
        void DeleteBook(Isbn isbn, bool removeDependencies = false);
        IEnumerable<Book> GetAllBooks();

        void AddCustomer(Customer customer);
        Customer GetCustomer(int id);
        void DeleteCustomer(Customer customer, bool removeDependencies = false);
        IEnumerable<Customer> GetAllCustomers();

        void AddBookCopy(BookCopy bookCopy);
        BookCopy GetBookCopy(int id);
        void DeleteBookCopy(BookCopy bookCopy, bool removeDependencies = false);
        IEnumerable<BookCopy> GetAllBookCopies();
        void AddEvent(Event eve);
        IEnumerable<Event> GetAllEvents();

    }
}
