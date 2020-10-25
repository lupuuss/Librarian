using Librarian.model.data;
using System;
using System.Collections.Generic;


namespace Librarian.model
{
    class DataRepository : IDataRepository
    {
        private DataContext _dataContext;
        public void AddBook(Book position)
        {
            _dataContext.books.Add(position.Isbn, position);
        }

        public void AddCustomer(Customer customer)
        {
            _dataContext.customers.Add(customer);
        }

        public void DeleteBook(Book position)
        {
            _dataContext.books.Remove(position.Isbn);
        }

        public void DeleteCustomer(Customer customer)
        {
            _dataContext.customers.Remove(customer);
        }

        public IEnumerable<Book> GetAllBooks()
        {
            return _dataContext.books.Values; 
        }

        public IEnumerable<Customer> GetAllCustomers()
        {
            return _dataContext.customers; 
        }

        public Book GetBook(Isbn isbn)
        {
            return _dataContext.books[isbn];
        }

        public Customer GetCustomer(Guid guid)
        {
            return _dataContext.customers.Find(customer => customer.Guid.Equals(guid)); 
        }


        public void UpdateBook(Isbn isbn, Book position)
        {
            throw new NotImplementedException();
        }

        public void UpdateCustomer(Guid guid, Customer customer)
        {
            throw new NotImplementedException();
        }

      
    }
}
