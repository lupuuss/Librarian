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
    
        [TestMethod]
        public void PaymentEventConstructor_NegativeOrZeroAmount_ThrowsException()
        {
            Assert.ThrowsException<ArgumentException>(
                () => new PaymentEvent(DateTime.Now, null, new Random().Next(1, 100000) * -1)
                );

            Assert.ThrowsException<ArgumentException>(
                () => new PaymentEvent(DateTime.Now, null, 0)
                );
        }

        [TestMethod]
        public void PaymentEventConstructor_PositiveAmount_SuccesfulCreation()
        {
            new PaymentEvent(DateTime.Now, null, new Random().Next(1, 100000));
        }

        [TestMethod]
        public void ReturnBookEvent_NegativeAmount_ThrowsException()
        {
            Assert.ThrowsException<ArgumentException>(
                () => new ReturnBookEvent(null, null, DateTime.Now, new Random().Next(1, 100000) * -1, PaymentCause.Postponed)
                );
        }

        [TestMethod]
        public void ReturnBookEvent_NotNoneCauseAndZeroAmount_ThrowsException()
        {
            Assert.ThrowsException<ArgumentException>(
                () => new ReturnBookEvent(null, null, DateTime.Now, new Random().Next(1, 100000), PaymentCause.None)
                );
        }

        [TestMethod]
        public void ReturnBookEvent_ProperArguments_SuccessfulCreation()
        {
            new ReturnBookEvent(null, null, DateTime.Now);

            new ReturnBookEvent(null, null, DateTime.Now, 10.0, PaymentCause.Postponed);

            new ReturnBookEvent(null, null, DateTime.Now, 20.0, PaymentCause.DamagedBook);
        }

    }
}
