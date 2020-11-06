using Librarian.model.data;
using Librarian.model.data.events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Librarian.model
{
    internal class DataContext
    {

        internal List<Customer> customers
        { get; private set; }

        internal Dictionary<Isbn, Book> books
        { get; private set; }

        internal ObservableCollection<Event> events
        { get; private set; }

        internal ObservableCollection<BookCopy> bookCopies
        { get; private set; }

    }
}
