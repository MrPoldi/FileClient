using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Classes
{
    class FileManager
    {
        internal List<DiscElement> EnumerateElements(string path)
        {
            MyDirectory myDir = new MyDirectory(path);
            List<DiscElement> discElements = new List<DiscElement>();
            List<DiscElement> myFiles = new List<DiscElement>();

            discElements.AddRange(myDir.GetMyDirectories());
            //myFiles.AddRange(myDir.GetMyFiles());
            discElements.AddRange(myDir.GetMyFiles());
            //discElements.AddRange(myFiles);
            return discElements;
        }
    }
}
