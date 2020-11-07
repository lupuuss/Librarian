using System;

namespace Librarian.Model.Data.Exceptions
{

    public class DataException : Exception
    {
        public DataException(string message) : base(message) { }
    }

    public class InvalidDataException : DataException
    {
        public InvalidDataException(string message) : base(message) { }
    }

    public class DataNotExistsException : DataException
    {
        public DataNotExistsException() : base("Data doesn't exist!") { }
    }

    public class DataNotRemovedException : DataException
    {
        public DataNotRemovedException() : base("Data could not be removed! Probably already doesn't exist!") { }
    }

    public class DataAlreadyExistsException : DataException
    {
        public DataAlreadyExistsException() : base("Data already exists!") { }
    }
}
