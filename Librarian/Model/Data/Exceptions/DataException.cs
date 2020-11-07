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

    public class UnsafeDataRemoveException : DataException
    {
        public UnsafeDataRemoveException(
            string referenceDetails
            ) : base("Unsafe delete! Use removeDependencies param" +
                "to enable cascade removal! Dependencies found: " + referenceDetails) { }
    }

    public class CustomerHasArreasException : DataException
    {
        public CustomerHasArreasException(string message) : base(message) { }
    }

    public class BookCopyLentException : DataException
    {
        public BookCopyLentException() : base("Book copy is already lent and cannot be removed!") { }
    }

    public class DataAlreadyExistsException : DataException
    {
        public DataAlreadyExistsException() : base("Data already exists!") { }
    }
}
