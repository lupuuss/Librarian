using Librarian.Model.Data;
using Librarian.Model.Data.Events;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Librarian.Model.Filler
{
    public class ConstDataFiller : IDataFiller
    {
        private List<Customer> _customers = new List<Customer>();
        private Dictionary<Isbn, Book> _books = new Dictionary<Isbn, Book>();
        private ObservableCollection<Event> _events = new ObservableCollection<Event>();
        private ObservableCollection<BookCopy> _bookCopies = new ObservableCollection<BookCopy>();

        public ConstDataFiller(
            List<Customer> customers = null,
            List<Book> books = null, 
            List<Event> events = null,
            List<BookCopy> bookCopies = null
            )
        {

            if (customers != null)
            {
                _customers = customers;
            }

            if (books != null)
            {
                _books = books.ToDictionary(x => x.Isbn, x => x);
            }

            if (bookCopies != null)
            {
                _bookCopies = new ObservableCollection<BookCopy>(bookCopies);
            }

            if (events != null)
            {
                _events = new ObservableCollection<Event>(events);
            }
        } 

        public void Fill(DataContext context)
        {
            context.customers.AddRange(_customers);

            foreach (var bookCopy in _bookCopies)
            {
                context.bookCopies.Add(bookCopy);
            }

            foreach (var eve in _events)
            {
                context.events.Add(eve); 
            }

            foreach (var pair in _books)
            {
                context.books[pair.Key] = pair.Value; 
            }
        }
    }
}
