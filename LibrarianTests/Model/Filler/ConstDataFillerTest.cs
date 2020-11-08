using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Librarian.Model;
using Librarian.Model.Filler;
using Librarian.Model.Data;
using System.Collections.Generic;
using Librarian.Model.Data.Events;
using System.Linq;

namespace LibrarianTests.Model.Filler
{
    [TestClass]
    public class ConstDataFillerTest
    {
        [TestMethod]
        public void Fill_Always_InsertsPassedDataToDataContext()
        {
            DataContext context = new DataContext();

            var customers = new List<Customer>()
            {
                new Customer("Janusz", "Kowalski", new Address("", "", "", "")),
                new Customer("Janusz", "Nowak", new Address("", "", "", "")),
                new Customer("Andrzej", "Nowak", new Address("", "", "", ""))
            };

            var books = new List<Book>()
            {
                new Book(new Isbn("978-3-16-148410-0"), "1", "1"),
                new Book(new Isbn("978-3-16-148427-0"), "2", "2"),
                new Book(new Isbn("978-3-16-148422-0"), "3", "3"),
                new Book(new Isbn("978-3-16-148498-0"), "4", "4")
            };

            var bookCopies = new List<BookCopy>()
            {
                new BookCopy(books[0], BookCopy.States.New, 10.0),
                new BookCopy(books[0], BookCopy.States.Good, 11.0),
                new BookCopy(books[1], BookCopy.States.Good, 12.0),
                new BookCopy(books[2], BookCopy.States.Used, 13.0),
                new BookCopy(books[3], BookCopy.States.Good, 14.0)
            };

            var events = new List<Event>()
            {
                new LendBookEvent(bookCopies[0], customers[0], DateTime.Parse("2/3/2020 9:00:00")),
                new LendBookEvent(bookCopies[2], customers[1], DateTime.Parse("4/3/2020 9:00:00")),
                new LendBookEvent(bookCopies[3], customers[2], DateTime.Parse("5/3/2020 9:00:00")),
                new LendBookEvent(bookCopies[1], customers[2], DateTime.Parse("9/3/2020 9:00:00")),
                new LendBookEvent(bookCopies[4], customers[2], DateTime.Parse("17/3/2020 9:00:00"))
            };

            ConstDataFiller dataFiller = new ConstDataFiller(
                customers: customers,
                books: books,
                bookCopies: bookCopies,
                events: events
                );

            dataFiller.Fill(context);

            var actualCustomers = context.customers;
            var actualBooks = context.books;
            var actualBookCopies = context.bookCopies;
            var actualEvents = context.events;

            CollectionAssert.AreEqual(customers, actualCustomers);

            CollectionAssert.AreEqual(books, actualBooks.Values);

            CollectionAssert.AreEqual(bookCopies, actualBookCopies.ToList());

            CollectionAssert.AreEqual(events, actualEvents);

        }
    }
}
