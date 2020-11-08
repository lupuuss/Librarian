using System;

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
