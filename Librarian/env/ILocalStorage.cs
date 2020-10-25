using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace Librarian.env
{
    public class DirectoryNotCreatedException : Exception
    {
        public DirectoryNotCreatedException(Exception cause) 
            : base("Directory could not be created!", cause) { }
    }

    public interface ILocalStorage
    {
        bool DirectoryExists(string dir);

        void CreateDirectory(string dir);
    }
}
