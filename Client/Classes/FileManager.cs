using System.Collections.Generic;

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
            discElements.AddRange(myDir.GetMyFiles());
            return discElements;
        }
    }
}
