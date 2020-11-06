namespace Librarian.Model.Data
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
            return string.Format("{{ Street: {0}, PostalCode: {1}, City: {2}, Country: {3} }}", Street, PostalCode, City, Country);
        }
    }
}
