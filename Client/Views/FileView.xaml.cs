using System.Windows;
using System.Windows.Controls;
using Client.Classes;

namespace Client.Views
{
    //simple file view
    public partial class FileView : UserControl
    {
        private MyFile myFile;

        public FileView(MyFile myFile)
        {
            this.myFile = myFile;
            InitializeComponent();
            FileName.Text = myFile.Name;
            FileCreationTime.Text = myFile.CreationDate.ToString();
        }

        //file selection delegate/event on ButtonClick
        public delegate void fileSelect(MyFile file);
        public event fileSelect fileSelection;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            fileSelection.Invoke(myFile);
        }
    }
}
