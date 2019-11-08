using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Client.Classes
{
    public class MyDirectory : DiscElement
    {
        public MyDirectory(string path) : base(path)
        {

        }

        public List<MyFile> GetMyFiles()
        {
            List<MyFile> myFiles = new List<MyFile>();
            string[] filesPath = Directory.EnumerateFiles(Path).ToArray();

            foreach (string file in filesPath)
            {
                myFiles.Add(new MyFile(file));
            }

            return myFiles;
        }

        public List<MyDirectory> GetMyDirectories()
        {
            List<MyDirectory> myDirectories = new List<MyDirectory>();
            string[] directoryPaths = Directory.EnumerateDirectories(Path).ToArray();

            foreach (string directory in directoryPaths)
            {
                myDirectories.Add(new MyDirectory(directory));
            }

            return myDirectories;
        }
    }
}
