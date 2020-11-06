namespace Librarian.Model.Data
{
    public class Customer
    {
        public string Name
        { get; private set; }

        public string LastName
        { get; private set; }

        public Address Address
        { get; private set; }

        public Customer(string name, string lastName, Address address)
        {
            Name = name;
            LastName = lastName;
            Address = address;
        }

        public override string ToString()
        {
            return string.Format("{{ Name: {0}; LastName: {1}; Address: {2}; }}", Name, LastName, Address.ToString());
        }
    }
}
