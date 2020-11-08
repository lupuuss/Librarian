using Librarian.Model;
using Librarian.Model.Data;
using Librarian.Model.Data.Events;
using Librarian.Model.Data.Exceptions;
using Librarian.Model.Date;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Librarian.Logic
{

    public class DataServiceException : Exception
    {
        public DataServiceException(Exception cause)
            : base("DataService task failed! Cause: " + cause.GetType().Name, cause) { }
    }

    public class DataService : IDataService
    {
        private readonly IDataRepository _dataRepository;
        private readonly IDateProvider _dateProvider;

        public int LendLimit
        { get; set; } = 60;

        public double PostponedPricePerDay
        { get; set; } = 0.5;

        public Dictionary<BookCopy.States, double> PaymentsModifiers
        { get; private set; } = new Dictionary<BookCopy.States, double>
        {
            { BookCopy.States.New, 100 },
            { BookCopy.States.Good, 80 },
            { BookCopy.States.Used, 60 },
            { BookCopy.States.Damaged, 0 },
            { BookCopy.States.NeedReplacement, 0 }
        };

        public DataService(IDataRepository dataRepository, IDateProvider dateProvider)
        {
            _dataRepository = dataRepository;
            _dateProvider = dateProvider;
        }

        public void CreateCustomer(string name, string lastName, Address address)
        {
            var customer = new Customer(name, lastName, address);

            _dataRepository.AddCustomer(customer); 
        }

        public IEnumerable<Customer> GetAllCustomers(string customerNameQuery = null)
        {

            if (customerNameQuery == null)
            {
                return _dataRepository.GetAllCustomers();
            }

            return _dataRepository.GetAllCustomers()
                .Where(c => (c.Name + " " + c.LastName)
                .Contains(customerNameQuery));
        }

        public void LendBook(Customer customer, BookCopy bookCopy)
        {
            var lend = new LendBookEvent(bookCopy, customer, _dateProvider.Now());

            try
            {
                _dataRepository.AddEvent(lend);
            } catch (EventException e)
            {
                throw new DataServiceException(e);
            }
        }

        public void ReturnBook(Customer customer, BookCopy bookCopy)
        {
            var date = _dateProvider.Now();
            var corespondingLend = _dataRepository
                .GetAllEvents()
                .Where(e => e is LendBookEvent)
                .Cast<LendBookEvent>()
                .Where(e => e.Customer == customer && e.Copy == bookCopy)
                .OrderByDescending(e => e.Date)
                .FirstOrDefault();

            if (corespondingLend == null)
            {
                throw new DataServiceException(
                    new Exception("No coresponding lend in repository! Book is not lent!")
                    );
            }

            try
            {

                var betweenDates = (date - corespondingLend.Date).Days;

                if (betweenDates < LendLimit)
                {
                    _dataRepository.AddEvent(new ReturnBookEvent(bookCopy, customer, date));
                    return;
                }

                var price = ((double)betweenDates - LendLimit) * PostponedPricePerDay;

                _dataRepository.AddEvent(
                    new ReturnBookEvent(bookCopy, customer, date, price, PaymentCause.Postponed)
                    );

            } catch (EventException e)
            {
                throw new DataServiceException(e);
            }
        }

        public void ReturnDamagedBook(Customer customer, BookCopy bookCopy)
        {
            var price = bookCopy.BasePrice * (PaymentsModifiers[bookCopy.State] / 100.0);

            var retunBook = new ReturnBookEvent(
                bookCopy,
                customer, 
                _dateProvider.Now(),
                price,
                PaymentCause.DamagedBook
                );

            try
            {
                _dataRepository.AddEvent(retunBook);
            } catch (EventException e)
            {
                throw new DataServiceException(e);
            }
        }

        public IEnumerable<Event> GetCustomerHistory(Customer customer)
        {
            return _dataRepository.GetAllEvents()
                .Where(e => e.Customer == customer)
                .OrderByDescending(e => e.Date);
         }

        public IEnumerable<Event> GetEvents(DateTime? fromDate = null, DateTime? toDate = null)
        {

            var result = _dataRepository.GetAllEvents();

            if (fromDate != null)
            {
                result = result.Where(e => e.Date > fromDate);
            }

            if (toDate != null)
            {
                result = result.Where(e => e.Date < toDate); 
            }

            return result.OrderByDescending(e => e.Date);
        }

        public Dictionary<Book, int> GetBooks()
        {
            var booksDic = new Dictionary<Book, int>();

            foreach (Book book in _dataRepository.GetAllBooks())
            {
                var bookCount = _dataRepository.GetAllBookCopies()
                    .Where(c => c.Book == book)
                    .Count();

                booksDic.Add(book, bookCount);
            }

            return booksDic;
        }

        public IEnumerable<BookCopy> GetAllCopies(Isbn isbn)
        {
            return _dataRepository.GetAllBookCopies()
                   .Where(c => c.Book.Isbn == isbn)
                   .OrderByDescending(c => c.State);

        }

        public void AddBook(Isbn isbn, string name, string author)
        {
            var book = new Book(isbn, name, author);

            try
            {
                _dataRepository.AddBook(book);
            } catch (DataException e)
            {
                throw new DataServiceException(e);
            }
        }

        public void AddBookCopy(Book book, BookCopy.States state, double price)
        {
            var bookCopy = new BookCopy(book, state, price);
            try
            {
                _dataRepository.AddBookCopy(bookCopy);
            } catch (DataException e)
            {
                throw new DataServiceException(e);
            }

        }

        public void AddPayment(Customer customer, double amount)
        {
            try
            {
                var payment = new PaymentEvent(_dateProvider.Now(), customer, amount);

                _dataRepository.AddEvent(payment);

            } catch (Exception e)
            {
                throw new DataServiceException(e);
            }
        }

        public double CheckBalance(Customer customer)
        {

            var deptCount = _dataRepository
                .GetAllEvents()
                .Where(e => e is ReturnBookEvent)
                .Cast<ReturnBookEvent>()
                .Where(e => e.Customer == customer)
                .Sum(e => e.RequiredPayment);

            var paidCount = _dataRepository
                .GetAllEvents()
                .Where(e => e is PaymentEvent)
                .Cast<PaymentEvent>()
                .Where(e => e.Customer == customer)
                .Sum(e => e.Amount);

            return paidCount - deptCount;
        }
    }
}
