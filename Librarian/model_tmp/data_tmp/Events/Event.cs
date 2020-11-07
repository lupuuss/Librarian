using System;

namespace Librarian.Model.Data.Events
{
    public abstract class Event
    {
        public DateTime Date
        { get; private set; }

        public Event(DateTime date)
        {
            Date = date;
        }
    }
}
