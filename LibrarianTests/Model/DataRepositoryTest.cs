using Librarian.Model;
using Librarian.Model.Data;
using Librarian.Model.Data.Exceptions;
using Librarian.Model.Filler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Librarian.Model.Data.Events;
using System;
using Librarian.Model.Date;

namespace LibrarianTests.Model
{
    [TestClass]
    public class DataRepositoryTest
    {

        private DataRepository _repo;
        private List<Book> _booksInDataFiller;
        private List<Customer> _customersInDataFiller;
        private List<BookCopy> _bookCopiesInDataFiller;
        private List<Event> _eventsInDataFiller;
        private SystemDateProvider _dateProvider;

        [TestInitialize]
        public void Initialize()
        {
            _dateProvider = new SystemDateProvider();

            _booksInDataFiller = new List<Book>()
            {
                 new Book(new Isbn("978-3-16-148410-0"), "The Da Vinci Code", "Dan Brown"),
                 new Book(new Isbn("978-3-16-148427-0"), "The Alchemist", "Paulo Coelho"),
                 new Book(new Isbn("978-3-16-148422-0"), "A Study in Scarlet", "Arthur Conan Doyle"),
                 new Book(new Isbn("978-3-16-148498-0"), "Animal Farm", "George Orwell")
            };

            _customersInDataFiller = new List<Customer>()
            {
                new Customer("Jan", "Kowalski", new Address("street", "11-222", "city", "country")),
                new Customer("Adam", "Nowak", new Address("street2", "22-333", "city2", "country2"))
            };

            _bookCopiesInDataFiller = new List<BookCopy>()
            {
                new BookCopy(_booksInDataFiller[0], BookCopy.States.Good, 100),
                new BookCopy(_booksInDataFiller[3], BookCopy.States.Good, 50),
                new BookCopy(_booksInDataFiller[2], BookCopy.States.Damaged, 33),
                new BookCopy(_booksInDataFiller[2], BookCopy.States.NeedReplacement, 33)
            };

            _eventsInDataFiller = new List<Event>()
            {
                new LendBookEvent(_bookCopiesInDataFiller[0], _customersInDataFiller[0], DateTime.ParseExact("15/03/2018","dd/MM/yyyy", null))
            };


        }


        [TestMethod]
        public void AddCustomer_CustomerNotInRepository_AddsToRepository()
        {
            _repo = new DataRepository(new ConstDataFiller());

            var customer = new Customer("Name", "LastName", new Address("street", "00-000", "city", "country"));
            _repo.AddCustomer(customer);
            var actual = _repo.GetAllCustomers().Count();

            Assert.AreEqual(1, actual);

        }

        public void AddCustomer_CustomerAlreadyInRepository_ExceptionThrown()
        {
            _repo = new DataRepository(new ConstDataFiller(customers: _customersInDataFiller));

            Assert.ThrowsException<DataAlreadyExistsException>(
                () => _repo.AddCustomer(_customersInDataFiller[0])
                );
        }

        [TestMethod]
        public void AddBook_BookNotInRepository_BookAdded()
        {
            _repo = new DataRepository(new ConstDataFiller(books: _booksInDataFiller));

            var book = new Book(new Isbn("978-3-16-118410-0"), "Sample", "Sample Author");

            _repo.AddBook(book);

            var actual = _repo.GetBook(book.Isbn);

            Assert.AreEqual(book, actual);
        }

        [TestMethod]
        public void AddBook_BookInTheRepository_ExceptionThrown()
        {
            _repo = new DataRepository(new ConstDataFiller(books: _booksInDataFiller));

            Assert.ThrowsException<DataAlreadyExistsException>(
                () => _repo.AddBook(_booksInDataFiller.First())
                );
        }

        [TestMethod]
        public void GetAllBooks_Always_ReturnsEveryAddedBook()
        {
            _repo = new DataRepository(new ConstDataFiller(books: _booksInDataFiller));

            var actual = _repo.GetAllBooks();

            CollectionAssert.AreEqual(
                _booksInDataFiller,
                (System.Collections.ICollection)actual
                );
        }

        [TestMethod]
        public void DeleteBook_BookInTheRepositoryWithNoDependencies_BookRemoved()
        {
            _repo = new DataRepository(new ConstDataFiller(books: _booksInDataFiller));

            _repo.DeleteBook(_booksInDataFiller[2].Isbn);
            _booksInDataFiller.RemoveAt(2);

            var actual = (System.Collections.ICollection)_repo.GetAllBooks();

            CollectionAssert.AreEqual(
                _booksInDataFiller,
                actual
                );
        }

        [TestMethod]
        public void DeleteBook_BookNotInTheRepository_ExceptionThrown()
        {
            _repo = new DataRepository(new ConstDataFiller(books: _booksInDataFiller));

            var book = new Book(new Isbn("978-3-77-118410-0"), "Sample", "Sample Author");

            Assert.ThrowsException<DataNotRemovedException>(
                () => _repo.DeleteBook(book.Isbn)
                );

        }

        [TestMethod]
        public void GetBook_BookInTheRepository_BookReturned()
        {
            _repo = new DataRepository(new ConstDataFiller(books: _booksInDataFiller));

            var actual = _repo.GetBook(_booksInDataFiller[3].Isbn);

            Assert.AreEqual(
                _booksInDataFiller[3],
                actual
                );
        }

        [TestMethod]
        public void GetBook_BookNotInTheRepository_ExceptionThrown()
        {
            _repo = new DataRepository(new ConstDataFiller(books: _booksInDataFiller));

            var book = new Book(new Isbn("978-3-77-118410-0"), "Sample", "Sample Author");

            Assert.ThrowsException<DataNotExistsException>(
                () => _repo.GetBook(book.Isbn)
                );
        }
        [TestMethod]
        public void AddBookCopy_BookCopyNotInTheRepository_BookCopyAdded()
        {
            _repo = new DataRepository(new ConstDataFiller(books: _booksInDataFiller));

            var bookCopy = new BookCopy(_booksInDataFiller[0], BookCopy.States.Good, 100);

            _repo.AddBookCopy(bookCopy);

            var actual = _repo.GetBookCopy(0);

            Assert.AreEqual(bookCopy, actual);
        }

        [TestMethod]
        public void AddBookCopy_BookInTheRepositoryAlreadyExists_ExceptionThrown()
        {
            _repo = new DataRepository(new ConstDataFiller(books: _booksInDataFiller));

            var bookCopy = new BookCopy(_booksInDataFiller[0], BookCopy.States.Good, 100);

            _repo.AddBookCopy(bookCopy);

            Assert.ThrowsException<DataAlreadyExistsException>(
                () => _repo.AddBookCopy(bookCopy)
                );
        }

        [TestMethod]
        public void AddBookCopy_BookDoesNotExistsInTheRepository_ExceptionThrown()
        {
            _repo = new DataRepository(new ConstDataFiller(books: _booksInDataFiller));

            var Book = new Book(new Isbn("978-3-77-118111-0"), "Sample2", "Sample Author2");
            var bookCopy = new BookCopy(Book, BookCopy.States.Good, 100);

            Assert.ThrowsException<InvalidDataException>(
                () => _repo.AddBookCopy(bookCopy)
                );
        }
        [TestMethod]
        public void GetBookCopy_BookIdDoesNotExist_ExceptionThrown()
        {
            _repo = new DataRepository(new ConstDataFiller(books: _booksInDataFiller));

            Assert.ThrowsException<DataNotExistsException>(
                () => _repo.GetBookCopy(1)
                );
        }

        [TestMethod]
        public void DeleteBookCopy_BookCopyInTheRepositoryWithNoDependencies_BookCopyRemoved()
        {

            _repo = new DataRepository(new ConstDataFiller(books: _booksInDataFiller,
                                                           bookCopies: _bookCopiesInDataFiller));

            _repo.DeleteBookCopy(_bookCopiesInDataFiller[0]);
            _bookCopiesInDataFiller.RemoveAt(0);

            var actual = (System.Collections.ICollection)_repo.GetAllBookCopies();

            CollectionAssert.AreEqual(
                _bookCopiesInDataFiller,
                actual
                );
        }

        [TestMethod]
        public void DeleteBook_BookHaveDepentedEvents_ExceptionThrown()
        {
            _repo = new DataRepository(new ConstDataFiller(books: _booksInDataFiller,
                                                           customers: _customersInDataFiller,
                                                           bookCopies: _bookCopiesInDataFiller,
                                                           events: _eventsInDataFiller));

            Assert.ThrowsException<UnsafeDataRemoveException>(
                () => _repo.DeleteBook(_booksInDataFiller[0].Isbn)
                );
        }

        [TestMethod]
        public void DeleteBookCopy_BookCopyHaveDepentedEvents_ExceptionThrown()
        {
            _repo = new DataRepository(new ConstDataFiller(books: _booksInDataFiller,
                                                           customers: _customersInDataFiller,
                                                           bookCopies: _bookCopiesInDataFiller,
                                                           events: _eventsInDataFiller));

            Assert.ThrowsException<UnsafeDataRemoveException>(
                () => _repo.DeleteBookCopy(_bookCopiesInDataFiller[0])
                );
        }

        [TestMethod]
        public void DeleteBook_BookHaveDepentedEventsForceDeletion_AllDependendEventsAreRemoved()
        {
            _repo = new DataRepository(new ConstDataFiller(books: _booksInDataFiller,
                                                           customers: _customersInDataFiller,
                                                           bookCopies: _bookCopiesInDataFiller,
                                                           events: _eventsInDataFiller));

            _repo.DeleteBook(_booksInDataFiller[0].Isbn, true);

            var actual = _repo.GetAllEvents().Count();
            Assert.AreEqual(0, actual);

            actual = _repo.GetAllBookCopies().Where(c => c.Book == _booksInDataFiller[0]).Count();
           
            Assert.AreEqual(0, actual);

            actual = _repo.GetAllBooks().Where(b => b == _booksInDataFiller[0]).Count();

            Assert.AreEqual(0, actual);

        }
        [TestMethod]
        public void DeleteBookCopy_BookCopyHaveDepentedEventsForceDeletion_AllDependendEventsAreRemoved()
        {
            _repo = new DataRepository(new ConstDataFiller(books: _booksInDataFiller,
                                                           customers: _customersInDataFiller,
                                                           bookCopies: _bookCopiesInDataFiller,
                                                           events: _eventsInDataFiller));

            _repo.DeleteBookCopy(_bookCopiesInDataFiller[0], true);
            var actual = _repo.GetAllEvents().Count();
            Assert.AreEqual(0, actual);
        }

        [TestMethod]
        public void DeleteCustomer_CustomerWithUnreturnedBook_DeletionCannotBeForced()
        {
            _repo = new DataRepository(new ConstDataFiller(books: _booksInDataFiller,
                                                           customers: _customersInDataFiller,
                                                           bookCopies: _bookCopiesInDataFiller,
                                                           events: _eventsInDataFiller));

            Assert.ThrowsException<CustomerHasArreasException>(
            () => _repo.DeleteCustomer(_customersInDataFiller[0], true)
            );

        }
        [TestMethod]
        public void DeleteCustomer_CustomerWithoutArreasForceDeletion_AllDependendEventsAreRemoved()
        {
            _repo = new DataRepository(new ConstDataFiller(books: _booksInDataFiller,
                                                           customers: _customersInDataFiller,
                                                           bookCopies: _bookCopiesInDataFiller,
                                                           events: _eventsInDataFiller));

            _bookCopiesInDataFiller[0].IsLent = true;

            _repo.AddEvent(new ReturnBookEvent(_bookCopiesInDataFiller[0],
                                               _customersInDataFiller[0],
                                               DateTime.ParseExact("27/02/2020", "dd/MM/yyyy", null)));

            _repo.DeleteCustomer(_customersInDataFiller[0], true);

            var actual = _repo.GetAllEvents().Count();
            Assert.AreEqual(0, actual);

        }

        [TestMethod]   
        public void DeleteCustomer_CustomerHasUnpaidFees_ExceptionThrown()
        {
            _repo = new DataRepository(new ConstDataFiller(books: _booksInDataFiller,
                                                           customers: _customersInDataFiller,
                                                           bookCopies: _bookCopiesInDataFiller,
                                                           events: _eventsInDataFiller));
            _bookCopiesInDataFiller[0].IsLent = true; 

            _repo.AddEvent(new ReturnBookEvent(_bookCopiesInDataFiller[0],
                                               _customersInDataFiller[0],
                                               DateTime.ParseExact("22/04/2018", "dd/MM/yyyy", null),
                                               3,
                                               PaymentCause.Postponed));
            
            Assert.ThrowsException<CustomerHasArreasException>(
                () => _repo.DeleteCustomer(_customersInDataFiller[0], true)
                );

        }
        [TestMethod]
        public void AddEvent_CustomerDoesNotExist_ExceptionThrown()
        {
            _repo = new DataRepository(new ConstDataFiller());

            var customer = new Customer("Jan", "Nowak", null);
            Assert.ThrowsException<InvalidEventException>( 
                () => _repo.AddEvent(new LendBookEvent(_bookCopiesInDataFiller[0], customer, DateTime.Now))
            ); 
        }

        [TestMethod]
        public void AddEvent_BookCopyDoesNotExist_ExceptionThrown()
        {
            _repo = new DataRepository(new ConstDataFiller(
                                        customers: _customersInDataFiller));

            var bookCopy = new BookCopy(_booksInDataFiller[0], BookCopy.States.Good, 50);
            Assert.ThrowsException<InvalidEventException>(
                () => _repo.AddEvent(new LendBookEvent(bookCopy, _customersInDataFiller[0], DateTime.Now))
            );
        }

        [TestMethod]
        public void AddEvent_LendingBookNotLent_AddsEventAndChangesBookIsLentToTrue()
        {
            _repo = new DataRepository(new ConstDataFiller(books: _booksInDataFiller,
                                                          customers: _customersInDataFiller,
                                                          bookCopies: _bookCopiesInDataFiller,
                                                          events: _eventsInDataFiller));

            var bookCopy = new BookCopy(_booksInDataFiller[3], BookCopy.States.Good, 20);
            _repo.AddBookCopy(bookCopy); 

            var eve = new LendBookEvent(bookCopy, _customersInDataFiller[1], DateTime.ParseExact("01/02/2020", "dd/MM/yyyy", null));

            _repo.AddEvent(eve);
            var actual = _repo.GetAllEvents().Contains(eve);
            Assert.AreEqual(true, actual);
            Assert.AreEqual(true, eve.Copy.IsLent);
        }

        [TestMethod]
        public void AddEvent_LendingBookInBadState_ExceptionThrown()
        {
            _repo = new DataRepository(new ConstDataFiller(books: _booksInDataFiller,
                                                          customers: _customersInDataFiller,
                                                          bookCopies: _bookCopiesInDataFiller,
                                                          events: _eventsInDataFiller));

            Assert.ThrowsException<InvalidEventException>(
                () => _repo.AddEvent(new LendBookEvent(_bookCopiesInDataFiller[2],
                                                       _customersInDataFiller[1],
                                                       DateTime.ParseExact("01/02/2020", "dd/MM/yyyy", null)))
                );

            Assert.ThrowsException<InvalidEventException>(
                () => _repo.AddEvent(new LendBookEvent(_bookCopiesInDataFiller[3],
                                                       _customersInDataFiller[1],
                                                       DateTime.ParseExact("01/02/2020", "dd/MM/yyyy", null)))
                );

        }
        [TestMethod]
        public void AddEvent_ReturningBookIsNotLent_ExceptionThrown()
        {
            _repo = new DataRepository(new ConstDataFiller(books: _booksInDataFiller,
                                                           customers: _customersInDataFiller,
                                                           bookCopies: _bookCopiesInDataFiller,
                                                           events: _eventsInDataFiller));

            var eve = new ReturnBookEvent(_bookCopiesInDataFiller[0],
                                          _customersInDataFiller[0],
                                          DateTime.ParseExact("22/03/2018", "dd/MM/yyyy", null));

            Assert.ThrowsException<InvalidEventException>(
                () => _repo.AddEvent(eve)
                );
        }

        [TestMethod]
        public void AddEvent_ReturningBookBookIsLent_AddsReturnBookEventAndChangesBookIsLentToFalse()
        {
            _bookCopiesInDataFiller[0].IsLent = true; 
            _repo = new DataRepository(new ConstDataFiller(books: _booksInDataFiller,
                                                          customers: _customersInDataFiller,
                                                          bookCopies: _bookCopiesInDataFiller,
                                                          events: _eventsInDataFiller));
            var eve = new ReturnBookEvent(_bookCopiesInDataFiller[0], _customersInDataFiller[0], DateTime.Now);
            _repo.AddEvent(eve);

            Assert.AreEqual(false, _bookCopiesInDataFiller[0].IsLent);
            Assert.AreEqual(true, _repo.GetAllEvents().Contains(eve));
        }

        [TestMethod]
        public void AddEvent_ReturningDamagedBookBookIsLent_ChangesBookStateToDamaged()
        {
            _bookCopiesInDataFiller[0].IsLent = true;
            _repo = new DataRepository(new ConstDataFiller(books: _booksInDataFiller,
                                                          customers: _customersInDataFiller,
                                                          bookCopies: _bookCopiesInDataFiller,
                                                          events: _eventsInDataFiller));

            var eve = new ReturnBookEvent(_bookCopiesInDataFiller[0], _customersInDataFiller[0], DateTime.Now, 20, PaymentCause.DamagedBook);
            _repo.AddEvent(eve);

            var actual = _bookCopiesInDataFiller[0].State;
            Assert.AreEqual(BookCopy.States.Damaged, actual);  
        }

        [TestMethod]
        public void AddEvent_ReturningDamagedBookWithNegativePayment_ExceptionThrown()
        {
            _bookCopiesInDataFiller[0].IsLent = true;
            _repo = new DataRepository(new ConstDataFiller(books: _booksInDataFiller,
                                                          customers: _customersInDataFiller,
                                                          bookCopies: _bookCopiesInDataFiller,
                                                          events: _eventsInDataFiller));

            var eve = new ReturnBookEvent(_bookCopiesInDataFiller[0],
                                          _customersInDataFiller[0],
                                          DateTime.Now,
                                          -20,
                                          PaymentCause.DamagedBook);
   

            Assert.ThrowsException<InvalidEventException>(
                () => _repo.AddEvent(eve)
                );

        }

        [TestMethod]
        public void AddEvent_ReturningBookWithPositivePaymentAndNoCause_ExceptionThrown()
        {
            _bookCopiesInDataFiller[0].IsLent = true;
            _repo = new DataRepository(new ConstDataFiller(books: _booksInDataFiller,
                                                          customers: _customersInDataFiller,
                                                          bookCopies: _bookCopiesInDataFiller,
                                                          events: _eventsInDataFiller));

            var eve = new ReturnBookEvent(_bookCopiesInDataFiller[0],
                                          _customersInDataFiller[0],
                                          DateTime.Now,
                                          20,
                                          PaymentCause.None);


            Assert.ThrowsException<InvalidEventException>(
                () => _repo.AddEvent(eve)
                );

        }

        [TestMethod]
        public void AddEvent_PaymentEventWithZeroOrNegativeAmout_ExceptionThrown()
        {
            _bookCopiesInDataFiller[0].IsLent = true;
            _repo = new DataRepository(new ConstDataFiller(books: _booksInDataFiller,
                                                          customers: _customersInDataFiller,
                                                          bookCopies: _bookCopiesInDataFiller,
                                                          events: _eventsInDataFiller));

            var eve = new PaymentEvent(DateTime.Now, _customersInDataFiller[0], 0);

            Assert.ThrowsException<InvalidEventException>(
                () => _repo.AddEvent(eve)
                );

            eve = new PaymentEvent(DateTime.Now, _customersInDataFiller[0], -15);

            Assert.ThrowsException<InvalidEventException>(
                () => _repo.AddEvent(eve)
                );

        }

        [TestMethod]
        public void AddEvent_PaymentEventWithPositiveAmout_AddedToEvents()
        {
            _bookCopiesInDataFiller[0].IsLent = true;
            _repo = new DataRepository(new ConstDataFiller(books: _booksInDataFiller,
                                                          customers: _customersInDataFiller,
                                                          bookCopies: _bookCopiesInDataFiller,
                                                          events: _eventsInDataFiller));

            var eve = new PaymentEvent(DateTime.Now, _customersInDataFiller[0], 15);

            _repo.AddEvent(eve);

            var actual = _repo.GetAllEvents().Contains(eve);
            Assert.AreEqual(true, actual);
             


        }

    }
}
