using Librarian.Logic;
using Librarian.Model;
using Librarian.Model.Date;
using Librarian.Model.Data;
using System;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Librarian.Model.Data.Events;
using Librarian.Model.Data.Exceptions;
using System.Diagnostics;

namespace LibrarianTests.Logic
{
    [TestClass]
    public class DataServiceTest
    {

        private readonly Mock<IDataRepository> _repoMock = new Mock<IDataRepository>();
        private readonly Mock<IDateProvider> _dateProviderMock = new Mock<IDateProvider>();

        private List<Customer> _customers;
        private List<Book> _books;
        private List<BookCopy> _bookCopies;
        private List<Event> _events;

        private DataService _dataService;
        private readonly DateTime _providedDate = DateTime.ParseExact("05/04/2020", "dd/MM/yyyy", null);

        [TestInitialize]
        public void Initialize()
        {

            _customers = new List<Customer>()
            {
                new Customer("Janusz", "Kowalski", new Address("", "", "", "")),
                new Customer("Janusz", "Nowak", new Address("", "", "", "")),
                new Customer("Andrzej", "Nowak", new Address("", "", "", "")),
                new Customer("Paweł", "Skakacz", new Address("", "", "", ""))
            };

            _books = new List<Book>()
            {
                new Book(new Isbn("978-3-16-148410-0"), "1", "1"),
                new Book(new Isbn("978-3-16-148427-0"), "2", "2"),
                new Book(new Isbn("978-3-16-148422-0"), "3", "3"),
                new Book(new Isbn("978-3-16-148498-0"), "4", "4"),
                new Book(new Isbn("978-3-16-148498-0"), "5", "5")
            };

            _bookCopies = new List<BookCopy>()
            {
                new BookCopy(_books[0], BookCopy.States.New, 10.0),
                new BookCopy(_books[0], BookCopy.States.Good, 11.0),
                new BookCopy(_books[1], BookCopy.States.Good, 12.0),
                new BookCopy(_books[2], BookCopy.States.Used, 13.0),
                new BookCopy(_books[3], BookCopy.States.Good, 14.0),
                new BookCopy(_books[4], BookCopy.States.Good, 14.0)
            };

            _events = new List<Event>()
            {
                new LendBookEvent(_bookCopies[0], _customers[0], DateTime.ParseExact("02/03/2020", "dd/MM/yyyy", null)),
                new LendBookEvent(_bookCopies[2], _customers[1], DateTime.ParseExact("04/03/2020", "dd/MM/yyyy", null)),
                new LendBookEvent(_bookCopies[3], _customers[2], DateTime.ParseExact("05/03/2020", "dd/MM/yyyy", null)),
                new LendBookEvent(_bookCopies[1], _customers[2], DateTime.ParseExact("09/03/2020", "dd/MM/yyyy", null)),
                new LendBookEvent(_bookCopies[4], _customers[2], DateTime.ParseExact("17/03/2020", "dd/MM/yyyy", null)),
                new LendBookEvent(_bookCopies[5], _customers[3], DateTime.ParseExact("18/03/2020", "dd/MM/yyyy", null)),
                new ReturnBookEvent(_bookCopies[5], _customers[3], DateTime.ParseExact("19/03/2020", "dd/MM/yyyy", null), 100, PaymentCause.DamagedBook),
                new PaymentEvent(DateTime.ParseExact("20/03/2020", "dd/MM/yyyy", null), _customers[3], 110)
            };

            _bookCopies[0].IsLent = true;
            _bookCopies[2].IsLent = true;
            _bookCopies[3].IsLent = true;

            _dateProviderMock
                .Setup(x => x.Now())
                .Returns(() => _providedDate);

            _repoMock
                .Setup(repo => repo.GetAllCustomers())
                .Returns(() => _customers);

            _repoMock
                .Setup(repo => repo.GetAllEvents())
                .Returns(() => _events);

            _repoMock
                .Setup(repo => repo.GetAllBooks())
                .Returns(() => _books);

            _repoMock
                .Setup(repo => repo.GetAllBookCopies())
                .Returns(() => _bookCopies);
        }

        [TestMethod]
        public void AddCustomer_Always_AddsCustomer()
        {
            _dataService = new DataService(_repoMock.Object, _dateProviderMock.Object);

            var address = new Address("Street", "00-000", "City", "Country");

            _dataService.CreateCustomer("Name", "LastName", address);

            _repoMock.Verify(
                repo => repo.AddCustomer(
                    Match.Create<Customer>(
                        c => c.Name == "Name" && c.LastName == "LastName" && c.Address == address
                        )
                    ),
                Times.Once()
                );
        }

        [TestMethod]
        public void GetAllCustomers_NoQuery_ReturnsAllCustomers()
        {
            _dataService = new DataService(_repoMock.Object, _dateProviderMock.Object);

            var actual = _dataService.GetAllCustomers();

            CollectionAssert.AreEqual(_customers, (System.Collections.ICollection)actual);
        }

        [TestMethod]
        public void GetAllCustomers_Query_ReturnsFilteredCustomers()
        {
            _dataService = new DataService(_repoMock.Object, _dateProviderMock.Object);

            var actual = _dataService.GetAllCustomers("Janusz");

            CollectionAssert.AreEqual(
                new List<Customer> { _customers[0], _customers[1] },
                actual.ToList()
                );

            actual = _dataService.GetAllCustomers("Nowak");

            CollectionAssert.AreEqual(
                new List<Customer> { _customers[1], _customers[2] },
                actual.ToList()
                );

            actual = _dataService.GetAllCustomers("Janusz Nowak");

            CollectionAssert.AreEqual(
                new List<Customer> { _customers[1] },
                actual.ToList()
                );
        }

        [TestMethod]
        public void LendBook_RepositoryThrowsException_ThrowsException()
        {
            _repoMock
                .Setup(repo => repo.AddEvent(It.IsAny<Event>()))
                .Throws(new EventException("Test exception"));

            _dataService = new DataService(_repoMock.Object, _dateProviderMock.Object);

            var exception = Assert.ThrowsException<DataServiceException>(
                () => _dataService.LendBook(_customers[0], _bookCopies[0])
                );

            Assert.IsInstanceOfType(exception.InnerException, typeof(EventException));
        }

        [TestMethod]
        public void LendBook_RepositoryAcceptsInput_AddsNewLendBookEvent()
        {


            _dataService = new DataService(_repoMock.Object, _dateProviderMock.Object);

            _dataService.LendBook(_customers[0], _bookCopies[1]);

            Predicate<LendBookEvent> validator = e => 
                e.Customer == _customers[0] && 
                e.Copy == _bookCopies[1] && 
                e.Date == _providedDate;

            _repoMock.Verify(
                repo => repo.AddEvent(Match.Create<LendBookEvent>(validator)), 
                Times.Once()
                );
        }

        [TestMethod]
        public void ReturnBook_BookNotLendForThisCustomer_ThrowsExceptionWithCause()
        {
            _dataService = new DataService(_repoMock.Object, _dateProviderMock.Object);

                Assert.ThrowsException<DataServiceException>(
                () => _dataService.ReturnBook(_customers[2], _bookCopies[0])
                );

        }

        [TestMethod]
        public void ReturnBook_NotPostponed_AddsEventWithoutPayment()
        {
            _dataService = new DataService(_repoMock.Object, _dateProviderMock.Object);

            _dataService.LendLimit = 40;

            _dataService.ReturnBook(_customers[1], _bookCopies[2]);

            Predicate<ReturnBookEvent> validator = e =>
                   e.Customer == _customers[1] &&
                   e.Copy == _bookCopies[2] &&
                   e.Date == _providedDate &&
                   e.RequiredPayment == 0.0 &&
                   e.Cause == PaymentCause.None;

            _repoMock.Verify(repo => repo.AddEvent(Match.Create<ReturnBookEvent>(validator)), Times.Once());
        }

        [TestMethod]
        public void ReturnBook_Postponed_AddsEventWithPayment()
        {
            _dataService = new DataService(_repoMock.Object, _dateProviderMock.Object);

            _dataService.LendLimit = 20;
            _dataService.PostponedPricePerDay = 2.0;

            _dataService.ReturnBook(_customers[1], _bookCopies[2]);

            Predicate<ReturnBookEvent> validator = e =>
                e.Customer == _customers[1] &&
                e.Copy == _bookCopies[2] &&
                e.Date == _providedDate &&
                e.Cause == PaymentCause.Postponed &&
                e.RequiredPayment == 12 * _dataService.PostponedPricePerDay; // 32 days between events - 20 (lend limit)

            _repoMock.Verify(repo => repo.AddEvent(Match.Create<ReturnBookEvent>(validator)), Times.Once());
        }

        [TestMethod]
        public void ReturnBook_RepositoryThrowsException_ThrowsExceptionWithCause()
        {
            _repoMock
                .Setup(repo => repo.AddEvent(It.IsAny<Event>()))
                .Throws(new EventException("Test exception"));

            _dataService = new DataService(_repoMock.Object, _dateProviderMock.Object);

            var exception = Assert.ThrowsException<DataServiceException>(
                () => _dataService.ReturnBook(_customers[1], _bookCopies[2])
                );

            Assert.IsInstanceOfType(exception.InnerException, typeof(EventException));
        }

        [TestMethod]
        public void ReturnDamagedBook_RepositoryAcceptsInputAndBookStateNew_AddsEventWithBookDamage()
        {
            _dataService = new DataService(_repoMock.Object, _dateProviderMock.Object);

            _dataService.ReturnDamagedBook(_customers[0], _bookCopies[0]);

            Predicate<ReturnBookEvent> validator = e =>
                e.Customer == _customers[0] &&
                e.Copy == _bookCopies[0] &&
                e.Cause == PaymentCause.DamagedBook &&
                e.Date == _providedDate &&
                Utils.AreEqual(
                    e.RequiredPayment,
                    _bookCopies[0].BasePrice * _dataService.PaymentsModifiers[_bookCopies[0].State] / 100.0,

                    );

            _repoMock.Verify(repo => repo.AddEvent(Match.Create<ReturnBookEvent>(validator)), Times.Once());

        }

        [TestMethod]
        public void ReturnDamagedBook_RepositoryAcceptsInputAndBookStateGood_AddsEventWithBookDamage()
        {
            _dataService = new DataService(_repoMock.Object, _dateProviderMock.Object);

            _dataService.ReturnDamagedBook(_customers[2], _bookCopies[3]);

            Predicate<ReturnBookEvent> validator = e =>
                e.Customer == _customers[2] &&
                e.Copy == _bookCopies[3] &&
                e.Cause == PaymentCause.DamagedBook &&
                e.Date == _providedDate &&
                Utils.AreEqual(
                    e.RequiredPayment,
                    _bookCopies[3].BasePrice * _dataService.PaymentsModifiers[_bookCopies[3].State] / 100.0,
                    0.001
                    );


            _repoMock.Verify(repo => repo.AddEvent(Match.Create<ReturnBookEvent>(validator)), Times.Once());
        }

        [TestMethod]
        public void ReturnDamagedBook_RepositoryAcceptsInputAndBookStateUsed_AddsEventWithBookDamage()
        {
            _dataService = new DataService(_repoMock.Object, _dateProviderMock.Object);

            _dataService.ReturnDamagedBook(_customers[1], _bookCopies[2]);

            Predicate<ReturnBookEvent> validator = e =>
                e.Customer == _customers[1] &&
                e.Copy == _bookCopies[2] &&
                e.Cause == PaymentCause.DamagedBook &&
                e.Date == _providedDate &&
                Utils.AreEqual(
                    e.RequiredPayment, 
                    _bookCopies[2].BasePrice * _dataService.PaymentsModifiers[_bookCopies[2].State] / 100.0,
                    0.001
                    );

            _repoMock.Verify(repo => repo.AddEvent(Match.Create<ReturnBookEvent>(validator)), Times.Once());
        }

        [TestMethod]
        public void ReturnDamagedBook_RepositoryThrowsException_ThrowsException()
        {

            _repoMock
                .Setup(repo => repo.AddEvent(It.IsAny<Event>()))
                .Throws(new EventException("Test event exception!"));

            _dataService = new DataService(_repoMock.Object, _dateProviderMock.Object);

            var exception = Assert.ThrowsException<DataServiceException>(
                () => _dataService.ReturnDamagedBook(_customers[1], _bookCopies[2])
                );

            Assert.IsInstanceOfType(exception.InnerException, typeof(EventException));
        }

        [TestMethod]
        public void GetCustomerHistory_Always_ReturnsAllCustomerEventsInDateDescendingOrder()
        {
            _dataService = new DataService(_repoMock.Object, _dateProviderMock.Object);

            var actual = _dataService.GetCustomerHistory(_customers[0])
                .ToList();

            CollectionAssert.AreEqual(
                new List<Event> { _events[0] },
                actual
                );


            actual = _dataService.GetCustomerHistory(_customers[1])
                .ToList();

            CollectionAssert.AreEqual(
                new List<Event> { _events[1] },
                actual
                );

            actual = _dataService.GetCustomerHistory(_customers[2])
                .ToList();


            CollectionAssert.AreEqual(
                new List<Event> { _events[4], _events[3], _events[2] },
                actual
                );
        }
        
        [TestMethod]
        public void GetEvents_NoParameters_ReturnsAllEventsInDescendingOrder()
        {
            _dataService = new DataService(_repoMock.Object, _dateProviderMock.Object);

            var actual = _dataService.GetEvents().ToList();

            CollectionAssert.AreEqual(
                _events.OrderByDescending(e => e.Date).ToList(),
                actual
                );
        }

        [TestMethod]
        public void GetEvents_OnlyFromParam_ReturnsEventsAfterPassedDateInDescendingOrder()
        {
            _dataService = new DataService(_repoMock.Object, _dateProviderMock.Object);

            var actual = _dataService.GetEvents(fromDate: DateTime.ParseExact("07/03/2020", "dd/MM/yyyy", null)).ToList();

            CollectionAssert.AreEqual(
                new List<Event> { _events[7], _events[6], _events[5], _events[4], _events[3] },
                actual
                );
        }

        [TestMethod]
        public void GetEvents_OnlyToParam_ReturnsEventsBeforePassedDateInDescendingOrder()
        {
            _dataService = new DataService(_repoMock.Object, _dateProviderMock.Object);

            var actual = _dataService.GetEvents(toDate: DateTime.ParseExact("07/03/2020", "dd/MM/yyyy", null)).ToList();

            CollectionAssert.AreEqual(
                new List<Event> { _events[2], _events[1], _events[0] },
                actual
                );
        }

        [TestMethod]
        public void GetEvents_BothParams_ReturnsEventsBetweenPassedDatesInDescendingOrder()
        {
            _dataService = new DataService(_repoMock.Object, _dateProviderMock.Object);

            var actual = _dataService.GetEvents(
                fromDate: DateTime.ParseExact("07/03/2020", "dd/MM/yyyy", null),
                toDate: DateTime.ParseExact("10/03/2020", "dd/MM/yyyy", null)
                ).ToList();

            CollectionAssert.AreEqual(
                new List<Event> { _events[3] },
                actual
                );
        }

        [TestMethod]
        public void GetBooks_Always_ReturnsBooksWithCopiesCount()
        {
            _dataService = new DataService(_repoMock.Object, _dateProviderMock.Object);

            var actualBooks = _dataService.GetBooks();

            Assert.AreEqual(2, actualBooks[_books[0]]);

            Assert.AreEqual(1, actualBooks[_books[1]]);

            Assert.AreEqual(1, actualBooks[_books[2]]);

            Assert.AreEqual(1, actualBooks[_books[3]]);
        }
    
        [TestMethod]
        public void GetAllCopies_Always_ReturnAllCopiesForPassedIsbnInStateDescendingOrder()
        {
            _dataService = new DataService(_repoMock.Object, _dateProviderMock.Object);

            var actual = _dataService.GetAllCopies(_books[0].Isbn);

            CollectionAssert.AreEqual(
                new List<BookCopy> { _bookCopies[0], _bookCopies[1] },
                actual.ToList()
                );

            actual = _dataService.GetAllCopies(_books[1].Isbn);

            CollectionAssert.AreEqual(
                new List<BookCopy> { _bookCopies[2] },
                actual.ToList()
                );

            actual = _dataService.GetAllCopies(_books[2].Isbn);

            CollectionAssert.AreEqual(
                new List<BookCopy> { _bookCopies[3] },
                actual.ToList()
                );

            actual = _dataService.GetAllCopies(_books[3].Isbn);

            CollectionAssert.AreEqual(
                new List<BookCopy> { _bookCopies[4] },
                actual.ToList()
                );
        }
    
    
        [TestMethod]
        public void AddBook_RepositoryAcceptsInput_AddsBook()
        {
            _dataService = new DataService(_repoMock.Object, _dateProviderMock.Object);

            var isbn = new Isbn("979-3-16-148410-0");

            _dataService.AddBook(isbn, "Title", "Author");

            Predicate<Book> validator = e =>
                e.Isbn == isbn &&
                e.Name == "Title" &&
                e.Author == "Author";

            _repoMock.Verify(
                repo => repo.AddBook(Match.Create<Book>(validator)),
                Times.Once()
                );
        }

        [TestMethod]
        public void AddBook_RepositoryThrowsException_ThrowsException()
        {

            _repoMock
                .Setup(repo => repo.AddBook(It.IsAny<Book>()))
                .Throws(new DataException("Test data exception!"));

            _dataService = new DataService(_repoMock.Object, _dateProviderMock.Object);

            var isbn = new Isbn("979-3-16-148410-0");

            var exception = Assert.ThrowsException<DataServiceException>(
                () => _dataService.AddBook(isbn, "Title", "Author")
                );

            Assert.IsInstanceOfType(exception.InnerException, typeof(DataException));

            Predicate<Book> validator = e =>
                e.Isbn == isbn &&
                e.Name == "Title" &&
                e.Author == "Author";

            _repoMock.Verify(
                repo => repo.AddBook(Match.Create<Book>(validator)),
                Times.Once()
                );
        }

        [TestMethod]
        public void AddBookCopy_RepositoryAcceptsInput_AddsBookCopy()
        {
            _dataService = new DataService(_repoMock.Object, _dateProviderMock.Object);

            _dataService.AddBookCopy(_books[0], BookCopy.States.New, 10.0);

            Predicate<BookCopy> validator = e =>
                e.Book == _books[0] &&
                e.State == BookCopy.States.New &&
                e.BasePrice == 10.0;

            _repoMock.Verify(
                repo => repo.AddBookCopy(Match.Create<BookCopy>(validator)),
                Times.Once()
                );
        }

        [TestMethod]
        public void AddBookCopy_RepositoryThrowsException_ThrowsException()
        {
            _repoMock
               .Setup(repo => repo.AddBookCopy(It.IsAny<BookCopy>()))
               .Throws(new DataException("Test data exception!"));

            _dataService = new DataService(_repoMock.Object, _dateProviderMock.Object);

            var exception = Assert.ThrowsException<DataServiceException>(
                () => _dataService.AddBookCopy(_books[0], BookCopy.States.New, 10.0)
                );

            Assert.IsInstanceOfType(exception.InnerException, typeof(DataException));

            Predicate<BookCopy> validator = e =>
                e.Book == _books[0] &&
                e.State == BookCopy.States.New &&
                e.BasePrice == 10.0;

            _repoMock.Verify(
                repo => repo.AddBookCopy(Match.Create<BookCopy>(validator)),
                Times.Once()
                );
        }

        [TestMethod]
        public void AddPayment_ExceptionOccurs_ThrowsException()
        {

            _repoMock
                .Setup(repo => repo.AddEvent(It.IsAny<Event>()))
                .Throws(new EventException("Event test exception!"));

            _dataService = new DataService(_repoMock.Object, _dateProviderMock.Object);

            var exception = Assert.ThrowsException<DataServiceException>(
                () => _dataService.AddPayment(_customers.First(), 100.0)
                );

            Assert.IsInstanceOfType(exception.InnerException, typeof(EventException));
        }

        [TestMethod]
        public void AddPayment_RepositoryAcceptsInput_AddPayment()
        {

            _dataService = new DataService(_repoMock.Object, _dateProviderMock.Object);

            _dataService.AddPayment(_customers.First(), 100.0);

            Predicate<PaymentEvent> validator = e =>
                e.Date == _providedDate &&
                e.Amount == 100.0 &&
                e.Customer == _customers.First();

            _repoMock.Verify(
                repo => repo.AddEvent(Match.Create<PaymentEvent>(validator)),
                Times.Once()
                );
        }

        [TestMethod]
        public void CheckBalance_NonZeroAmounts_ReturnsProperBalance()
        {

            _dataService = new DataService(_repoMock.Object, _dateProviderMock.Object);

            var actual = _dataService.CheckBalance(_customers[3]);

            Assert.AreEqual(10, actual);
        }

        [TestMethod]
        public void CheckBalance_ZeroAmounts_ReturnsZeroBalance()
        {

            _dataService = new DataService(_repoMock.Object, _dateProviderMock.Object);

            foreach (Customer customer in _customers.GetRange(0, 3))
            {
                var actual = _dataService.CheckBalance(customer);

                Assert.AreEqual(0, actual);
            }
        }
    }
}
