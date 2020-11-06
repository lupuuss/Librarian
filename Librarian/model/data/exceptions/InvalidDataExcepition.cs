using System;

namespace Librarian.Model.Data.Exceptions
{
    class InvalidDataExcepition : Exception
    {
        InvalidDataExcepition(string message) : base(message) { }
    }
}
