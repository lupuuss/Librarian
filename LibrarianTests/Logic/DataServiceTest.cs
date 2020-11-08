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
        private readonly DateTime _providedDate = DateTime.Parse("5/4/2020 9:00:00");

        [TestInitialize]
        public void Initialize()
        {

            _customers = new List<Customer>()
            {
                new Customer("Janusz", "Kowalski", new Address("", "", "", "")),
                new Customer("Janusz", "Nowak", new Address("", "", "", "")),
                new Customer("Andrzej", "Nowak", new Address("", "", "", ""))
            };

            _books = new List<Book>()
            {
                new Book(new Isbn("978-3-16-148410-0"), "1", "1"),
                new Book(new Isbn("978-3-16-148427-0"), "2", "2"),
                new Book(new Isbn("978-3-16-148422-0"), "3", "3"),
                new Book(new Isbn("978-3-16-148498-0"), "4", "4")
            };

            _bookCopies = new List<BookCopy>()
            {
                new BookCopy(_books[0], BookCopy.States.Good, 10.0),
                new BookCopy(_books[0], BookCopy.States.Good, 11.0),
                new BookCopy(_books[1], BookCopy.States.Good, 12.0),
                new BookCopy(_books[2], BookCopy.States.Good, 13.0),
                new BookCopy(_books[3], BookCopy.States.Good, 14.0)
            };

            _events = new List<Event>()
            {
                new LendBookEvent(_bookCopies[0], _customers[0], DateTime.Parse("2/3/2020 9:00:00")),
                new LendBookEvent(_bookCopies[2], _customers[1], DateTime.Parse("4/3/2020 9:00:00")),
                new LendBookEvent(_bookCopies[3], _customers[2], DateTime.Parse("5/3/2020 9:00:00"))
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

            _repoMock.Verify(
                repo => repo.AddEvent(
                    Match.Create<LendBookEvent>(e => e.Customer == _customers[0] && e.Copy == _bookCopies[1] && e.Date == _providedDate)
                    ),
                Times.Once()
                );
        }

        [TestMethod]
        public void ReturnBook_BookNotLendForThisCustomer_ThrowsExceptionWithCause()
        {
            _dataService = new DataService(_repoMock.Object, _dateProviderMock.Object);

            var exception = Assert.ThrowsException<DataServiceException>(
                () => _dataService.ReturnBook(_customers[2], _bookCopies[0])
                );

            Assert.IsInstanceOfType(exception.InnerException, typeof(Exception));
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
                e.Cause == PaymentCause.Postponed &&
                e.Date == _providedDate &&
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
    }
}
