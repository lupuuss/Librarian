using Librarian.Model.Data;
using Librarian.Model.Data.Events;
using Librarian.Model.Data.Exceptions;
using Librarian.Model.Filler;
using System.Collections.Generic;
using System.Linq;

namespace Librarian.Model
{
    public class DataRepository : IDataRepository
    {
        private DataContext _dataContext;

        public DataRepository(IDataFiller dataFiller)
        {
            _dataContext = new DataContext();
            dataFiller.Fill(_dataContext);
        }

        public void AddBook(Book position)
        {
            if (_dataContext.books.ContainsKey(position.Isbn))
            {
                throw new DataAlreadyExistsException();
            }

            _dataContext.books.Add(position.Isbn, position);
        }

        public Book GetBook(Isbn isbn)
        {
            CheckBookIsbn(isbn);

            return _dataContext.books[isbn];
        }

        public void UpdateBook(Isbn isbn, Book position)
        {
            CheckBookIsbn(isbn);
            _dataContext.books[isbn] = position;
        }

        private void CheckBookIsbn(Isbn isbn)
        {
            if (!_dataContext.books.ContainsKey(isbn))
            {
                throw new DataNotExistsException();
            }
        }

        public void DeleteBook(Isbn isbn)
        {
         
            var result = _dataContext.books.Remove(isbn);

            if (!result)
            {
                throw new DataNotRemovedException();
            }
        }

        public IEnumerable<Book> GetAllBooks()
        {
            return _dataContext.books.Values;
        }

        public void AddBookCopy(BookCopy bookCopy)
        {
            if (_dataContext.bookCopies.Contains(bookCopy))
            {
                throw new DataAlreadyExistsException();
            }

            CheckBookCopyReference(bookCopy);

            _dataContext.bookCopies.Add(bookCopy);
        }

        public BookCopy GetBookCopy(int id)
        {

            CheckBookCopyId(id);
            return _dataContext.bookCopies[id];
        }
        
        public void UpdateBookCopy(int id, BookCopy bookCopy)
        {
            CheckBookCopyId(id);

            CheckBookCopyReference(bookCopy);

            _dataContext.bookCopies[id] = bookCopy;
        }
        
        private void CheckBookCopyReference(BookCopy bookCopy)
        {
            if (!_dataContext.books.ContainsKey(bookCopy.Book.Isbn))
            {
                throw new InvalidDataException("Book copy refers to book that is not in repository!");
            }
        }

        private void CheckBookCopyId(int id)
        {
            if (id > _dataContext.bookCopies.Count())
            {
                throw new DataNotExistsException();
            }
        }

        public void DeleteBookCopy(BookCopy bookCopy)
        {

            var result = _dataContext.bookCopies.Remove(bookCopy);

            if (!result)
            {
                throw new DataNotRemovedException();
            }

        }

        public IEnumerable<BookCopy> GetAllBookCopies()
        {
            return _dataContext.bookCopies;
        }

        public void AddCustomer(Customer customer)
        {

            if (_dataContext.customers.Contains(customer))
            {
                throw new DataAlreadyExistsException();
            }

            _dataContext.customers.Add(customer);
        }
        
        public Customer GetCustomer(int id)
        {

            CheckCustomerId(id);
            return _dataContext.customers[id];
        }

        public void UpdateCustomer(int id, Customer customer)
        {

            CheckCustomerId(id);
            _dataContext.customers[id] = customer;
        }

        private void CheckCustomerId(int id)
        {
            if (id > _dataContext.customers.Count())
            {
                throw new DataNotExistsException();
            }
        }
        
        public void DeleteCustomer(Customer customer)
        {
            var result = _dataContext.customers.Remove(customer);

            if (!result)
            {
                throw new DataNotRemovedException();
            }
        }

        public IEnumerable<Customer> GetAllCustomers()
        {
            return _dataContext.customers;
        }

        public void AddEvent(Event eve)
        {

            switch (eve)
            {
                case LendBookEvent lendBookEvent:
                    IsValid(lendBookEvent);

                    lendBookEvent.Copy.IsLent = true;

                    break;
                case ReturnBookEvent returnBookEvent:
                    IsValid(returnBookEvent);

                    returnBookEvent.Copy.IsLent = false;

                    if (returnBookEvent.Cause == PaymentCause.DamagedBook)
                    {
                        returnBookEvent.Copy.State = BookCopy.States.Damaged;
                    }

                    break;
                case PaymentEvent payment:
                    IsValid(payment);
                    break;
                default:
                    throw new UnrecognizedEventException();
            }

            _dataContext.events.Add(eve);
        }
        
        private void IsValid(LendBookEvent lendBookEvent)
        {
            CheckBookEvent(lendBookEvent);

            if (lendBookEvent.Copy.State == BookCopy.States.Damaged ||
                lendBookEvent.Copy.State == BookCopy.States.NeedReplacement)
            {
                throw new InvalidEventException("Book is in a bad state - cannot be lent!");
            }
            if (lendBookEvent.Copy.IsLent)
            {
                throw new InvalidEventException("Book is already lent.");
            }
        }

        private void IsValid(ReturnBookEvent returnBookEvent)
        {
            CheckBookEvent(returnBookEvent);

            if (!returnBookEvent.Copy.IsLent)
            {
                throw new InvalidEventException("Cannot return book which is not lent");
            }


            var lastCorespondingLend = _dataContext.events
                .AsEnumerable()
                .Where(eve => eve is LendBookEvent)
                .Cast<LendBookEvent>()
                .Where(eve => eve.Copy == returnBookEvent.Copy && eve.Customer == returnBookEvent.Customer)
                .OrderByDescending(eve => eve.Date)
                .FirstOrDefault();

            if (lastCorespondingLend == null)
            {
                throw new InvalidEventException("No coresponding lend event!");
            }
            else if (lastCorespondingLend.Date >= returnBookEvent.Date)
            {
                throw new InvalidEventException("Return date should after lend date!");
            }
        }

        private void IsValid(PaymentEvent paymentEvent)
        {
            CheckCustomerExistance(paymentEvent.Customer);

            if (paymentEvent.Amount > 0)
            {
                throw new InvalidEventException("Payment amount must be positive number!");
            }
        }

        private void CheckBookEvent(BookEvent bookEvent)
        {
            CheckCustomerExistance(bookEvent.Customer);
            CheckBookCopyExistance(bookEvent.Copy);
        }

        private void CheckCustomerExistance(Customer customer)
        {
            if (!_dataContext.customers.Contains(customer))
            {
                throw new InvalidEventException("Customer does not exist.");
            }
        }

        private void CheckBookCopyExistance(BookCopy bookCopy)
        {
            if (!_dataContext.bookCopies.Contains(bookCopy))
            {
                throw new InvalidEventException("Book copy does not exist.");
            }
        }

        public IEnumerable<Event> GetAllEvents()
        {
            return _dataContext.events;
        }
    }
}
