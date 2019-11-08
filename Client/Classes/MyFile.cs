using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Classes
{
    public class MyFile : DiscElement
    {

        public MyFile(string path) : base(path)
        {

        }

        public string Extension
        {
            get { return System.IO.Path.GetExtension(this.Path); }
        }
    }
}
