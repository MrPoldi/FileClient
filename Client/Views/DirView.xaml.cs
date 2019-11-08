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
