using Librarian.Logic;
using Librarian.Model;
using Librarian.Model.Date;
using Librarian.Model.Data;
using System;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace LibrarianTests.Logic
{
    [TestClass]
    public class DataServiceTest
    {

        private readonly Mock<IDataRepository> _repoMock = new Mock<IDataRepository>();
        private readonly Mock<IDateProvider> _dateProviderMock = new Mock<IDateProvider>();

        private readonly List<Customer> _customers = new List<Customer>
        {
            new Customer("Janusz", "Kowalski", new Address("", "", "", "")),
            new Customer("Janusz", "Nowak", new Address("", "", "", "")),
            new Customer("Andrzej", "Nowak", new Address("", "", "", ""))
        };

        private DataService _dataService;

        [TestInitialize]
        public void Initialize()
        {

            _dateProviderMock
                .Setup(x => x.Now())
                .Returns(() => DateTime.Parse("5/4/2005 9:00:00"));

            _repoMock
                .Setup(x => x.GetAllCustomers())
                .Returns(() => _customers);
        }

        [TestMethod]
        public void AddCustomer_Always_AddsCustomer()
        {
            _dataService = new DataService(_repoMock.Object, _dateProviderMock.Object);

            var address = new Address("Street", "00-000", "City", "Country");

            _dataService.CreateCustomer("Name", "LastName", address);

            _repoMock.Verify(
                repo => repo.AddCustomer(
                    Match.Create<Customer>(
                        c => c.Name == "Name" && c.LastName == "LastName" && c.Address == address
                        )
                    ), 
                Times.Once()
                );
        }

        [TestMethod]
        public void GetAllCustomers_NoQuery_ReturnsAllCustomers()
        {
            _dataService = new DataService(_repoMock.Object, _dateProviderMock.Object);

            var actual = _dataService.GetAllCustomers();

            CollectionAssert.AreEqual(_customers, (System.Collections.ICollection)actual);
        }

        public void GetAllCustomers_Query_ReturnsFilteredCustomers()
        {
            _dataService = new DataService(_repoMock.Object, _dateProviderMock.Object);

            var actual = _dataService.GetAllCustomers("Janusz");

            CollectionAssert.AreEqual(
                new List<Customer> { _customers[0], _customers[1] },
                (System.Collections.ICollection)actual
                );

            actual = _dataService.GetAllCustomers("Nowak");

            CollectionAssert.AreEqual(
                new List<Customer> { _customers[1], _customers[2] },
                (System.Collections.ICollection)actual
                );

            actual = _dataService.GetAllCustomers("Janusz Nowak");

            CollectionAssert.AreEqual(
                new List<Customer> { _customers[1] },
                (System.Collections.ICollection)actual
                );
        }
    }
}
