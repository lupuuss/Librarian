using System;
using System.Collections.Generic;
using System.Text;

namespace Librarian.model.filler
{
    interface IDataFiller
    {
        void Fill(DataContext context);
    }
}
