using System;
using System.Collections.Generic;
using System.Text;

namespace Librarian.model.filler
{
    public interface IDataFiller
    {
        void Fill(DataContext context);
    }
}
