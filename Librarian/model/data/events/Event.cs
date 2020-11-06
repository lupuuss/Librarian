using System;

namespace Librarian.model.data.events
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
