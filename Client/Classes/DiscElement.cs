using System;
using System.IO;

namespace Client.Classes
{
    public abstract class DiscElement
    {
        private string path;
        private string name;
        private DateTime creationDate;

        public string Name
        {
            get { return name; }
        }

        public string Path
        {
            get { return path; }
        }

        public DateTime CreationDate
        {
            get { return creationDate; }
        }

        public DiscElement(string path)
        {
            this.path = path;
            name = path.Substring(path.LastIndexOf(@"\") + 1);
            creationDate = File.GetCreationTime(path);
        }        
    }
}
