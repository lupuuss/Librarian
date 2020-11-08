using Librarian.Model;
using Librarian.Model.Data;
using Librarian.Model.Data.Exceptions;
using Librarian.Model.Filler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System;

namespace LibrarianTests.Model
{
    [TestClass]
    public class DataRepositoryTest
    {

        private DataRepository _repo;
        private List<Book> _booksInDataFiller;
        private List<Customer> _customersInDataFiller;
        private List<BookCopy> _bookCopiesInDataFiller; 

        [TestInitialize]
        public void Initialize()
        {
            _booksInDataFiller = new List<Book>()
            {
                 new Book(new Isbn("978-3-16-148410-0"), "The Da Vinci Code", "Dan Brown"), 
                 new Book(new Isbn("978-3-16-148427-0"), "The Alchemist", "Paulo Coelho"),
                 new Book(new Isbn("978-3-16-148422-0"), "A Study in Scarlet", "Arthur Conan Doyle"),
                 new Book(new Isbn("978-3-16-148498-0"), "Animal Farm", "George Orwell")
            };

            _customersInDataFiller = new List<Customer>()
            {
                new Customer("Jan", "Kowalski", new Address("street", "postalCode", "city", "country")),
                new Customer("Adam", "Nowak", new Address("street2", "postalCode2", "city2", "country2"))
            };

            
        }

        [TestMethod]
        public void AddBook_BookNotInRepository_BookAdded()
        {
            _repo = new DataRepository(new ConstDataFiller(books: _booksInDataFiller));

            var book = new Book(new Isbn("978-3-16-118410-0"), "Sample", "Sample Author");

            _repo.AddBook(book);

            Assert.AreEqual(book, _repo.GetBook(book.Isbn));
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
        public void RemoveBook_BookInTheRepository_BookRemoved()
        {
            _repo = new DataRepository(new ConstDataFiller(books: _booksInDataFiller));

            _repo.DeleteBook(_booksInDataFiller[2].Isbn);
            _booksInDataFiller.RemoveAt(2);

            var actual = _repo.GetAllBooks();

            CollectionAssert.AreEqual(
                _booksInDataFiller,
                (System.Collections.ICollection)actual
                );
        }

        [TestMethod]
        public void RemoveBook_BookNotInTheRepository_ExceptionThrown()
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

            var actual = _booksInDataFiller[3];

            Assert.AreEqual(
                _repo.GetBook(actual.Isbn),
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

            Assert.AreEqual(bookCopy, _repo.GetBookCopy(0)); 
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
        public void DeleteBookCopy_BookCopyInTheRepository_BookCopyRemoved()
        {
            var bookCopy = new BookCopy(_booksInDataFiller[0], BookCopy.States.Good, 100);
            _bookCopiesInDataFiller = new List<BookCopy>()
            {
                bookCopy
            };
            _repo = new DataRepository(new ConstDataFiller(books: _booksInDataFiller, bookCopies: _bookCopiesInDataFiller));

           
            CollectionAssert.AreEqual(
                _bookCopiesInDataFiller,
                (System.Collections.ICollection)_repo.GetAllBookCopies()
                );
        }

        [TestMethod] 
        public void DeleteBook_BookHaveDepentedEvents_ExceptionThrown()
        {
            _repo = new DataRepository(new ConstDataFiller(books: _booksInDataFiller, customers: _customersInDataFiller));
        }
    }
}
