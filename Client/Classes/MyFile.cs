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

        public long Size
        {
            get { return new System.IO.FileInfo(this.Path).Length; }
        }
    }
}
