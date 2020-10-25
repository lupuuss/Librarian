using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Librarian.env
{
    class LocalStorage : ILocalStorage
    {
        public void CreateDirectory(string dir)
        {
            try
            {
                Directory.CreateDirectory(dir);
            } catch (Exception e)
            {
                throw new DirectoryNotCreatedException(e);
            }
        }

        public bool DirectoryExists(string dir)
        {
            return Directory.Exists(dir);
        }
    }
}
