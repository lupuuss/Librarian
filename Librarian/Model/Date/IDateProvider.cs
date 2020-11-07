using System;
using System.Collections.Generic;
using System.Text;

namespace Librarian.Model.Date
{
    public interface IDateProvider
    {
        DateTime Now();
    }
}
