using System;
using System.Text.RegularExpressions;

namespace Librarian
{
    public class Isbn : IEquatable<Isbn>
    {
        string _value;
        public Isbn(string isbn)
        {

            var regex = new Regex(@".*"); // TODO: Isbn Regex 

            if (!regex.Match(isbn).Success)
            {
                throw new ArgumentException("Passed value is not a valid ISBN!");
            }

            _value = isbn;
        }

        public bool Equals(Isbn other)
        {
            return _value == other._value;
        }
    }

    public class Book
    {

        public Isbn Isbn
        { get; private set; }

        public string Name
        { get; private set; }

        public string Author
        { get; private set; }

        public Book(Isbn isbn, string name, string author)
        {
            Isbn = isbn;
            Name = name;
            Author = author;
        }
    }
}
