using Librarian.model.data;
using Librarian.model.data.events;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace Librarian.model.filler
{
    public class ConstDataFiller : IDataFiller
    {
        public void Fill(DataContext context)
        {
            context.customers.Add(new Customer("Filip", "Kowalski", new Address("al. Politechniki 1", "90-924", "Łódź", "Poland")));
            context.customers.Add(new Customer("Damian", "Baczyński", new Address("al. Politechniki 2", "90-924", "Łódź", "Poland")));

            List<Book> tempBooks = new List<Book>
            {
                new Book(new Isbn("978-3-16-148410-0"), "The Da Vinci Code", "Dan Brown"),
                new Book(new Isbn("978-3-16-148427-0"), "The Alchemist", "Paulo Coelho"),
                new Book(new Isbn("978-3-16-148422-0"), "A Study in Scarlet", "Arthur Conan Doyle"),
                new Book(new Isbn("978-3-16-148498-0"), "Animal Farm", "George Orwell")
            };

            context.books.Add(tempBooks[0].Isbn, tempBooks[0]);
            context.books.Add(tempBooks[1].Isbn, tempBooks[1]);
            context.books.Add(tempBooks[2].Isbn, tempBooks[2]);
            context.books.Add(tempBooks[3].Isbn, tempBooks[3]);

            context.bookCopies.Add(new BookCopy(tempBooks[0], BookCopy.States.New, 60.50));
            context.bookCopies.Add(new BookCopy(tempBooks[0], BookCopy.States.Good, 60.50));
            context.bookCopies.Add(new BookCopy(tempBooks[0], BookCopy.States.Used, 60.50));
            context.bookCopies.Add(new BookCopy(tempBooks[1], BookCopy.States.New, 30.19));
            context.bookCopies.Add(new BookCopy(tempBooks[1], BookCopy.States.Damaged, 30.19));
            context.bookCopies.Add(new BookCopy(tempBooks[2], BookCopy.States.Damaged, 19.99));
            context.bookCopies.Add(new BookCopy(tempBooks[3], BookCopy.States.NeedReplacement, 15.20));
            context.bookCopies.Add(new BookCopy(tempBooks[3], BookCopy.States.Good, 15.20));


            context.events.Add(
                new LendBookEvent(context.bookCopies[0], context.customers[0], DateTime.Parse("28/5/2019 18:32:00"))
                );

            context.events.Add(
                new ReturnBookEvent(context.bookCopies[0], context.customers[0], DateTime.Parse("29/5/2019 17:10:00"))
            );

            context.events.Add(
                new LendBookEvent(context.bookCopies[1], context.customers[1], DateTime.Parse("28/5/2019 19:27:00"))
                );

            context.events.Add(
                new ReturnBookEvent(
                    context.bookCopies[1], 
                    context.customers[1], 
                    DateTime.Parse("28/5/2019 19:27:00"),
                    context.bookCopies[1].Price, 
                    PaymentCause.DamagedBook
                    )
            );

            context.events.Add(
                new PaymentEvent(
                    DateTime.Parse("28/5/2019 19:30:00"),
                    context.customers[1],
                    context.bookCopies[1].Price
                )
            );


            context.bookCopies[1].State = BookCopy.States.Damaged;

        }
    }
}
