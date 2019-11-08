using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Client.Classes;

namespace Client.Views
{
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

        public delegate void fileSelect(MyFile file);
        public event fileSelect fileSelection;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            fileSelection.Invoke(myFile);
        }
    }
}
