using System;

namespace Librarian.model.data
{
    public class Customer : IEquatable<Customer>
    {
        public Guid Guid
        { get; private set; }

        public string Name
        { get; private set; }

        public string LastName
        { get; private set; }

        public Customer(string name, string lastName, Guid guid)
        {
            Name = name;
            LastName = lastName;
            Guid = guid;
        }

        public Customer(string name, string lastName) : this(name, lastName, Guid.NewGuid()) { }

        public bool Equals(Customer other)
        {
            return Guid == other.Guid; 
        }
    }
}
