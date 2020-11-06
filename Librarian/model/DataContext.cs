using Librarian.model.data;
using Librarian.model.data.events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Librarian.model
{
    public class DataContext
    {

        internal List<Customer> customers
        { get; private set; } = new List<Customer>();

        internal Dictionary<Isbn, Book> books
        { get; private set; } = new Dictionary<Isbn, Book>();

        internal ObservableCollection<Event> events
        { get; private set; } = new ObservableCollection<Event>();

        internal ObservableCollection<BookCopy> bookCopies
        { get; private set; } = new ObservableCollection<BookCopy>();

    }
}
