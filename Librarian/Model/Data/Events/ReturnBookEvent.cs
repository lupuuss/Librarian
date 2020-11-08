using System;

namespace Librarian.Model.Data.Events
{
    public enum PaymentCause
    {
        /// <summary>
        /// No payment.
        /// </summary>
        None,

        /// <summary>
        /// Payment for the destruction of the book.
        /// </summary>
        DamagedBook,

        /// <summary>
        /// Payment for postponed return.
        /// </summary>
        Postponed
    }
    public class ReturnBookEvent : BookEvent
    {
        public double RequiredPayment
        { get; private set; } = 0.0;

        public PaymentCause Cause
        { get; private set; } = PaymentCause.None;

        public ReturnBookEvent(
            BookCopy copy,
            Customer customer,
            DateTime date
            ) : base(copy, customer, date)
        {
        }

        public ReturnBookEvent(
            BookCopy copy,
            Customer customer,
            DateTime date,
            double requiredPayment,
            PaymentCause cause
            ) : this(copy, customer, date)
        {

            RequiredPayment = requiredPayment;
            Cause = cause;
        }
    }
}
