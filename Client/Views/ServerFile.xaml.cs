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

namespace Client.Views
{
    public partial class ServerFile : UserControl
    {
        private String myString;

        public ServerFile(String myString)
        {
            this.myString = myString;
            InitializeComponent();
            FileName.Text = myString;
        }

        public delegate void fileSelect(String fileName);
        public event fileSelect fileSelection;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            fileSelection.Invoke(myString);
        }
    }
}
