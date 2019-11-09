using System.Windows;
using System.Windows.Controls;
using Client.Classes;

namespace Client.Views
{
    public partial class DirView : UserControl
    {
        private MyDirectory myDir;

        public DirView(MyDirectory myDir)
        {
            this.myDir = myDir;
            InitializeComponent();
            DirName.Text = myDir.Name;
            DirCreationTime.Text = myDir.CreationDate.ToString();
        }

        public delegate void directoryChange(string path);
        public event directoryChange dirChange;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            dirChange.Invoke(myDir.Path);
        }
    }
}
