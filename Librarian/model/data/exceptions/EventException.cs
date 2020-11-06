using System;
using System.Collections.Generic;
using System.Text;

namespace Librarian.model.data.exceptions
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
}
