namespace Librarian.Model.Data
{
    public class BookCopy
    {

        public enum States
        {
            /// <summary>
            /// Damaged by the customer e.g. ripped page.
            /// </summary>
            Damaged,

            /// <summary>
            /// Needs replacement, but not because of single customer damage, but because of long time exploatation
            /// </summary>
            NeedReplacement,

            /// <summary>
            /// Have significant traces of usage, but still can be used. May require replacement in the future.
            /// </summary>
            Used,

            /// <summary>
            /// Have minor traces of usage.
            /// </summary>
            Good,

            /// <summary>
            /// Brand new. No signs of usage.
            /// </summary>
            New
        }

        public States State
        { get; set; }

        public Book Book
        { get; private set; }

        public double BasePrice
        { get; private set; }

        public bool IsLent
        { get; set; } = false;

        public BookCopy(Book book, States state, double basePrice)
        {
            Book = book;
            State = state;
            BasePrice = basePrice;
        }
    }
}
