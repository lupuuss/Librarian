using System;

namespace Librarian.model.data.events
{
    public class LendBookEvent : BookEvent
    {
        public LendBookEvent(BookCopy copy, Customer customer, DateTime date) : base(copy, customer, date)
        {
        }
    }
}
