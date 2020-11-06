using System;

namespace Librarian.model.data
{
    public class BookCopy
    {
        /// <summary>
        /// State value represents percentage of a base price that need to be paid in case of customer damage.
        /// Dameged has -1 as its represent damaged book that is already paid.
        /// </summary>
        public enum States
        {
            /// <summary>
            /// Damaged by the customer e.g. ripped page.
            /// </summary>
            Damaged = -1,

            /// <summary>
            /// Needs replacement, but not because of single customer damage, but because of long time exploatation
            /// </summary>
            NeedReplacement = 0,

            /// <summary>
            /// Have significant traces of usage, but still can be used. May require replacement in the future.
            /// </summary>
            Used = 50,

            /// <summary>
            /// Have minor traces of usage.
            /// </summary>
            Good = 80,

            /// <summary>
            /// Brand new. No signs of usage.
            /// </summary>
            New = 100
        }

        public States State
        { get; set; }

        public Book Book
        { get; private set; }

        public double BasePrice
        { get; private set; }

        public bool IsLent
        { get;  set; } = false; 

        public double Price => BasePrice * (double) State / 100.0;

        public BookCopy(Book book, States state, double basePrice)
        {
            Book = book;
            State = state;
            BasePrice = basePrice;
        }
    }
}
