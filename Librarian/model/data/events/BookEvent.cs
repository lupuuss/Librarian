﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Librarian.model.data.events
{
    public abstract class BookEvent : Event
    {
        public BookCopy Copy
        { get; private set; }
        public Customer Customer
        { get; private set; }
        public BookEvent(BookCopy copy, Customer customer, DateTime date) : base(date)
        {
            Copy = copy;
            Customer = customer;
        }
    }
}
