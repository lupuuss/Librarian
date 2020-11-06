using System;
using System.Collections.Generic;
using System.Text;

namespace Librarian.model.data.events
{
    public class PaymentEvent : Event
    {

        public Customer Customer
        { get; private set; }
        public double Amount
        { get; private set; }

        public PaymentEvent(DateTime date, Customer customer, double amount) : base(date)
        {
            Customer = customer;
            Amount = amount;
        }
    }
}
