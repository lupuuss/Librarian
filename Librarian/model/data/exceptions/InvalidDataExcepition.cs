using System;
using System.Collections.Generic;
using System.Text;

namespace Librarian.model.data.exceptions
{
    class InvalidDataExcepition : Exception
    {
        InvalidDataExcepition(string message) : base(message) { }
    }
}
