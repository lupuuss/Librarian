using System;
using System.Linq;
using Librarian.Model;
using Librarian.Model.Filler;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LibrarianTests.Model.Filler
{
    [TestClass]
    public class RandomDataFillerTest
    {
        [TestMethod]
        public void Fill_Always_AddsApproperiateAmountOfData()
        {
            RandomDataFiller dataFiller = new RandomDataFiller(10, 10, 5, 3);

            DataContext context = new DataContext();

            dataFiller.Fill(context);

            Assert.AreEqual(10, context.customers.Count);
            Assert.AreEqual(10, context.books.Count);
            Assert.AreEqual(10 * 5, context.bookCopies.Count);
            Assert.AreEqual(10 * 3, context.events.Count);

            Assert.AreEqual(10, context.customers.Distinct().Count());
            Assert.AreEqual(10, context.books.Distinct().Count());
            Assert.AreEqual(10 * 5, context.bookCopies.Distinct().Count());
            Assert.AreEqual(10 * 3, context.events.Distinct().Count());
        }
    }
}
