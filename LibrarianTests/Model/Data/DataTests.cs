using System;
using Librarian.Model.Data;
using Librarian.Model.Data.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LibrarianTests.Model.Data
{
    [TestClass]
    public class DataTests
    {
        [TestMethod]
        public void IsbnConstructor_BadFormat_ThrowsException()
        {

            Assert.ThrowsException<ArgumentException>(
                () => new Isbn("978-3-16-14410-0")
                );

            Assert.ThrowsException<ArgumentException>(
                () => new Isbn("978-3-16-148410-")
                );

            Assert.ThrowsException<ArgumentException>(
                () => new Isbn("978-3-16-14841000")
                );

            Assert.ThrowsException<ArgumentException>(
                () => new Isbn("97803-16-14841000")
                );

            Assert.ThrowsException<ArgumentException>(
                () => new Isbn("978-3916-14841000")
                );

            Assert.ThrowsException<ArgumentException>(
                () => new Isbn("978-3916914841000")
                );
        }

        [TestMethod]
        public void IsbnConstructor_GoodFormat_SuccessfulCreation()
        {
            new Isbn("978-3-16-148410-0");
            new Isbn("000-0-00-000000-3");
            new Isbn("111-3-16-148410-2");
            new Isbn("222-0-36-140000-1");
        }

    }
}
