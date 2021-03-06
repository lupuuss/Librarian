﻿using System;

namespace Librarian.Model.Data.Events
{
    public class PaymentEvent : Event
    {

        public double Amount
        { get; private set; }

        public PaymentEvent(DateTime date, Customer customer, double amount) : base(customer, date)
        {
            
            Amount = amount;
        }
    }
}
