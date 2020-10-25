using Librarian.model.data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Librarian.model
{
    class DataContext
    {

        public List<Customer> customers
        { get; private set; }

        public Dictionary<Isbn, Book> books
        { get; private set; }

        public ObservableCollection<Lending> lendings
        { get; private set; }

        public ObservableCollection<BookCopy> bookCopies
        { get; private set; }

    }
}
