using System;

namespace Librarian.model.data
{
    public class ReturnDateBeforeLendingDateException : Exception
    {
        public ReturnDateBeforeLendingDateException() 
            : base("Book copy returning date must be after book was landed.") { }
    }

    public class Lending
    {
        public Guid Guid
        { get; private set; }
        public Customer Customer
        { get; private set; }
        
        public BookCopy BookCopy
        { get; private set; }

        public DateTime LendingDate
        { get; set; }

        private DateTime? _returnDate = null;

        public DateTime? ReturnDate
        { 
            get => _returnDate;
            set {
                if (value < LendingDate)
                {
                    throw new ReturnDateBeforeLendingDateException();
                }

                _returnDate = value;
            } 
        }

        public Lending(Customer customer, BookCopy bookCopy, DateTime lendingDate, Guid guid)
        {
            Customer = customer;
            BookCopy = bookCopy;
            Guid = guid;
            LendingDate = lendingDate;
            
        }

        public Lending(Customer customer, BookCopy bookCopy, DateTime lendingDate) : this(customer, bookCopy, lendingDate, Guid.NewGuid()) { }

        bool IsReturned()
        {
            return ReturnDate != null;
        }
    }
}
