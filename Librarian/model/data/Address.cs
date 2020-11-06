using System;
using System.Collections.Generic;
using System.Text;

namespace Librarian.model.data
{
    public class Address
    {
        public string Street { get; private set; }
        public string PostalCode { get; private set; }
        public string City { get; private set; }
        public string Country { get; private set; }

        public Address(string street, string postalCode, string city, string country)
        {
            Street = street;
            PostalCode = postalCode;
            City = city;
            Country = country;
        }

        public override string ToString()
        {
            return String.Format("{{ Street: {0}, PostalCode: {1}, City: {2}, Country: {3} }}", Street, PostalCode, City, Country);
        }
    }
}
