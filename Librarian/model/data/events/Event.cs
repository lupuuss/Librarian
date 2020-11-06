using System;

namespace Librarian.model.data.events
{

    public class EventException : Exception
    {
        public EventException(string message) : base(message) { }
    }

    public class InvalidEventException : EventException
    {
        public InvalidEventException(string cause) : base("Invalid event! Cause: " + cause) { }
    }

    public class UnrecognizedEventException : EventException 
    {
        public UnrecognizedEventException() : base("Unrecognized Event implementation!") { }
    }

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
