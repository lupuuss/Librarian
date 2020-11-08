using Librarian.Model;
using Librarian.Model.Data;
using Librarian.Model.Data.Events;
using Librarian.Model.Date;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Schema;

namespace Librarian.Logic
{

    public class DataServiceException : Exception
    {
        public DataServiceException(
            Exception cause
            ) : base("DataService task failed! Cause: " + cause.GetType().Name, cause) { }
    }

    public class DataService : IDataService
    {
        private readonly IDataRepository _dataRepository;
        private readonly IDateProvider _dateProvider;

        public int LendLimit
        { get; set; } = 60;

        public double PostponedPricePerDay
        { get; set; } = 0.5;

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

            _dataRepository.AddEvent(lend); 
        }

        public void ReturnBook(Customer customer, BookCopy bookCopy)
        {
            var date = _dateProvider.Now();
            var corespondingLend = _dataRepository
                .GetAllEvents()
                .Where(e => e is LendBookEvent)
                .Cast<LendBookEvent>()
                .OrderByDescending(e => e.Date)
                .FirstOrDefault();

            if (corespondingLend != null)
            {
                throw new NotImplementedException();
            }

            var betweenDates = (date - corespondingLend.Date).Days;

            if (betweenDates < LendLimit)
            {
                _dataRepository.AddEvent(new ReturnBookEvent(bookCopy, customer, date));
            }

            var price = ((double)betweenDates) * PostponedPricePerDay;

            _dataRepository.AddEvent(
                new ReturnBookEvent(bookCopy, customer, date, price, PaymentCause.Postponed)
                ); 
        }

        public void ReturnDamagedBook(Customer customer, BookCopy bookCopy)
        {
            var retunBook = new ReturnBookEvent(bookCopy, customer, _dateProvider.Now(), bookCopy.Price, PaymentCause.DamagedBook);
            _dataRepository.AddEvent(retunBook); 
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

            return result;
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
            _dataRepository.AddBook(book); 
        }

        public void AddBookCopy(Book book, BookCopy.States state, double price)
        {
            var bookCopy = new BookCopy(book, state, price);
            _dataRepository.AddBookCopy(bookCopy); 
        }
    }
}
