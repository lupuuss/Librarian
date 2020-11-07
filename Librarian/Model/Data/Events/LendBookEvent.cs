using System;

namespace Librarian.Model.Data.Events
{
    public class LendBookEvent : BookEvent
    {
        public LendBookEvent(BookCopy copy, Customer customer, DateTime date) : base(copy, customer, date)
        {
        }
    }
}
