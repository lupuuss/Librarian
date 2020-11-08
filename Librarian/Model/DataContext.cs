using Librarian.Model.Data;
using Librarian.Model.Data.Events;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Librarian.Model
{
    public class DataContext
    {

        public List<Customer> customers
        { get; private set; } = new List<Customer>();

        public Dictionary<Isbn, Book> books
        { get; private set; } = new Dictionary<Isbn, Book>();

        public ObservableCollection<Event> events
        { get; private set; } = new ObservableCollection<Event>();

        public ObservableCollection<BookCopy> bookCopies
        { get; private set; } = new ObservableCollection<BookCopy>();

    }
}
