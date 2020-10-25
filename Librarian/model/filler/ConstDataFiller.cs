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
            context.customers.Add(new data.Customer("Filip", "Kowalski", Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")));
            context.customers.Add(new data.Customer("Damian", "Baczyński", Guid.Parse("7c9e6679-7425-40de-944b-e07fc1f90ae7")));

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

            context.bookCopies.Add(new BookCopy(tempBooks[0], BookCopy.States.New, 60.50,
                Guid.Parse("31fb20b3-4c01-460b-8187-337f23c99927")));
            context.bookCopies.Add(new BookCopy(tempBooks[0], BookCopy.States.Good, 60.50,
                Guid.Parse("ffb7ed65-8f97-49ce-981a-330483fef569")));
            context.bookCopies.Add(new BookCopy(tempBooks[0], BookCopy.States.Used, 60.50,
                Guid.Parse("e0e39f37-2e35-4689-8ba5-24e49c27df8d")));
            context.bookCopies.Add(new BookCopy(tempBooks[1], BookCopy.States.New, 30.19,
                Guid.Parse("109a179a-9752-4309-b57b-c40af50748a6")));
            context.bookCopies.Add(new BookCopy(tempBooks[1], BookCopy.States.Damaged, 30.19,
                Guid.Parse("cec93f9d-eee0-46d5-8ba3-d0ed8f59e780")));
            context.bookCopies.Add(new BookCopy(tempBooks[2], BookCopy.States.Damaged, 19.99,
                Guid.Parse("a6104f6c-116e-4429-a007-d2f11ca2f0f1")));
            context.bookCopies.Add(new BookCopy(tempBooks[3], BookCopy.States.NeedReplacement, 15.20,
                Guid.Parse("4ec49343-6dd6-459e-af0f-ff0716ba12b9")));
            context.bookCopies.Add(new BookCopy(tempBooks[3], BookCopy.States.Good, 15.20,
                Guid.Parse("9e15640b-0f50-4766-994c-ca6e62ced9e9")));

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
