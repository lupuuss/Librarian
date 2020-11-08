using System;
using System.Linq;
using Librarian.Model.Data;
using Librarian.Model.Data.Events;

namespace Librarian.Model.Filler
{
    public class RandomDataFiller : IDataFiller
    {
        private readonly int _customersCount;
        private readonly int _booksCount;
        private readonly int _copiesPerBookCount;
        private readonly int _eventsPerUserCount;

        public RandomDataFiller(int customersCount, int booksCount, int copiesPerBookCount, int eventsPerUserCount)
        {
            _customersCount = customersCount;
            _booksCount = booksCount;
            _copiesPerBookCount = copiesPerBookCount;
            _eventsPerUserCount = eventsPerUserCount;
        }

        private string RandomString(int minLength, int maxLength)
        {
            Random random = new Random();
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            var randomString = Enumerable.Repeat(chars, random.Next(minLength, maxLength))
                .Select(s => s[random.Next(s.Length)])
                .ToArray();

            return new string(randomString);
        }

        private Isbn RandomIsbn()
        {

            var random = new Random();
            var str = random
                .Next(0, 999)
                .ToString()
                .PadLeft(paddingChar: '0', totalWidth: 3);

            str = str + "-" + random.Next(0, 9).ToString();

            str = str + "-" + random.Next(0, 99)
                .ToString()
                .PadLeft(paddingChar: '0', totalWidth: 2);
            str = str + "-" + random.Next(0, 999999)
                .ToString()
                .PadLeft(paddingChar: '0', totalWidth: 6);

            str = str + "-" + random.Next(0, 9).ToString();

            return new Isbn(str);
        }

        public void Fill(DataContext context)
        {

            Random random = new Random();

            for (int i = 0; i < _booksCount; i++)
            {

                var book = new Book(RandomIsbn(), RandomString(5, 10), RandomString(5, 15));

                context.books.Add(book.Isbn, book);
                for (int j = 0; j < _copiesPerBookCount; j++)
                {
                    context.bookCopies.Add(
                        new BookCopy(book, BookCopy.States.New, random.Next(5, 100))
                        );
                }
            }


            for (int i = 0; i < _customersCount; i++)
            {

                var name = RandomString(3, 10);
                var lastName = RandomString(3, 15);

                var street = RandomString(6, 15);
                var postalCodeEnum = Enumerable
                    .Repeat<Func<int>>(() => random.Next(0, 9), 5)
                    .Select(fun => fun().ToString());

                var postalCode = string.Join("", postalCodeEnum);

                postalCode = postalCode.Substring(0, 2) + "-" + postalCode.Substring(2, 3);

                var city = RandomString(5, 10);
                var country = RandomString(5, 10);

                context.customers.Add(
                    new Customer(name, lastName, new Address(street, postalCode, city, country))
                    );

                var date = DateTime.Now;

                for (int j = 0; j < _eventsPerUserCount; j++)
                {
                    date = date.AddDays(1);

                    var realityCheck = random.Next(0, 10);
                    var anyDone = false;

                    if (realityCheck % 2 == 0 && context.bookCopies.Where(c => !c.IsLent).Count() > 0)
                    {
                        var bookCopy = context.bookCopies.Where(c => !c.IsLent).First();
                        bookCopy.IsLent = true;

                        context.events.Add(new LendBookEvent(bookCopy, context.customers[i], date));
                        anyDone = true;
                    }

                    var lendings = context.events.Where(e => e is LendBookEvent)
                                   .Cast<LendBookEvent>()
                                   .Where(e => e.Copy.IsLent && e.Customer == context.customers[i])
                                   .ToList();

                    if (realityCheck % 2 == 1 && lendings.Count > 0) 
                    {
                        var lending = lendings.First();

                        context.events.Add(new ReturnBookEvent(lending.Copy, lending.Customer, date));
                        anyDone = true;
                    }

                    if (!anyDone)
                    {
                        context.events.Add(new PaymentEvent(date, context.customers[i], random.Next(10, 100)));
                    }
                }
            }
            
        }
    }
}
