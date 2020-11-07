using System;
using System.Collections.Generic;
using System.Text;

namespace Librarian.Model.Date
{
    public class SystemDateProvider : IDateProvider
    {
        public DateTime Now()
        {
            return DateTime.Now; 
        }
    }
}
