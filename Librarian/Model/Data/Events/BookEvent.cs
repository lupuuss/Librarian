using System;

namespace Librarian.Model.Data.Events
{
    public abstract class BookEvent : Event
    {
        public BookCopy Copy
        { get; private set; }
        
        public BookEvent(BookCopy copy, Customer customer, DateTime date) : base(customer, date)
        {
            Copy = copy;
        }
    }
}
