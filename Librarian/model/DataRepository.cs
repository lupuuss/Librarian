using Librarian.model.data;
using Librarian.model.filler;
using System;
using System.Collections.Generic;
using System.Data;

namespace Librarian.model
{
    class DataRepository : IDataRepository
    {
        private DataContext _dataContext;

        DataRepository(IDataFiller dataFiller)
        {
            _dataContext = new DataContext();
            dataFiller.Fill(_dataContext); 
        }

        public void AddBook(Book position)
        {
            _dataContext.books.Add(position.Isbn, position);
        }

        public Book GetBook(Isbn isbn)
        {
            return _dataContext.books[isbn];
        }

        public void UpdateBook(Isbn isbn, Book position)
        {
            _dataContext.books[isbn] = position;
        }

        public void DeleteBook(Book position)
        {
            _dataContext.books.Remove(position.Isbn);
        }

        public IEnumerable<Book> GetAllBooks()
        {
            return _dataContext.books.Values;
        }

        public void AddBookCopy(BookCopy bookCopy)
        {
            _dataContext.bookCopies.Add(bookCopy); 
        }

        public BookCopy GetBookCopy(int id)
        {
            return _dataContext.bookCopies[id];
        }
        public void UpdateBookCopy(int id, BookCopy bookCopy)
        {
            _dataContext.bookCopies[id] = bookCopy; 
        }

        public void DeleteBookCopy(BookCopy bookCopy)
        {
            _dataContext.bookCopies.Remove(bookCopy);
        }

        public IEnumerable<BookCopy> GetAllBookCopies()
        {
            return _dataContext.bookCopies;
        }

        public void AddCustomer(Customer customer)
        {
            _dataContext.customers.Add(customer);
        }
        public Customer GetCustomer(int id)
        {
            return _dataContext.customers[id];
        }

        public void UpdateCustomer(int id, Customer customer)
        {
            _dataContext.customers[id] = customer;
        }
        public void DeleteCustomer(Customer customer)
        {
            _dataContext.customers.Remove(customer);
        }

        public IEnumerable<Customer> GetAllCustomers()
        {
            return _dataContext.customers;
        }



    }
}
