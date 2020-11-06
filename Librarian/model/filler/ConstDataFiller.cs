using Librarian.model.data;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace Librarian.model.filler
{
    class ConstDataFiller : IDataFiller
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

            context.lendings.Add(new Lending(context.customers[0], context.bookCopies[1],
                DateTime.Parse("08 / 21 / 2019 08:22:00"), Guid.Parse("5f2a0406-bd37-42cd-b463-3a1f7591cc42")));
            context.lendings.Add(new Lending(context.customers[0], context.bookCopies[2],
                DateTime.Parse("05 / 28 / 2019 18:32:00"), Guid.Parse("3dfe021c-5fd1-408e-a609-44e75929c6d1")));
            context.lendings.Add(new Lending(context.customers[0], context.bookCopies[3],
                DateTime.Parse("03 / 25 / 2019 09:12:00"), Guid.Parse("12fde254-bfd9-4c59-ac23-7d0044552a16")));
            context.lendings.Add(new Lending(context.customers[1], context.bookCopies[2],
                DateTime.Parse("02 / 14 / 2019 11:15:00"), Guid.Parse("262ed0b6-ba0a-4850-a8f5-5ceeb5d96905")));
            context.lendings.Add(new Lending(context.customers[1], context.bookCopies[3],
                DateTime.Parse("01 / 12 / 2019 12:27:00"), Guid.Parse("9df582ee-ecbf-42af-9ae4-a24039dedeba")));
            context.lendings.Add(new Lending(context.customers[1], context.bookCopies[0],
                DateTime.Parse("12 / 11 / 2019 13:23:00"), Guid.Parse("84fc2829-b79c-4927-82ad-0785dda4afb7")));

        }
    }
}
