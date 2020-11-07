using System;

namespace Librarian.Model.Data.Events
{
    public abstract class Event
    {
        public DateTime Date
        { get; private set; }
        public Customer Customer
        { get; private set; }

        public Event(Customer customer, DateTime date)
        {
            Customer = customer; 
            Date = date;
        }
    }
}
