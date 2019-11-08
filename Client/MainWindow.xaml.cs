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
using Client.Views;
using System.IO;

namespace Client
{
    public partial class MainWindow : Window
    {
        private readonly FileManager myFileManager;
        private List<DiscElement> myDiscElements;
        private string currentPath;
        private DirectoryInfo dirInfo;

        public MainWindow()
        {
            this.myFileManager = new FileManager();
            InitializeComponent();
            currentPath = "D:\\";
            Refresh(currentPath);
        }

        private void Refresh(string path)
        {
            UserFiles.Items.Clear();
            dirInfo = new DirectoryInfo(path);
            myDiscElements = myFileManager.EnumerateElements(path);

            foreach (DiscElement discElement in myDiscElements)
            {
                if (discElement is MyFile)
                {
                    FileView fileView = new FileView((discElement as MyFile));
                    UserFiles.Items.Add(fileView);
                }

                else if (discElement is MyDirectory)
                {
                    DirectoryInfo dir = new DirectoryInfo(discElement.Path);
                    if (!dir.Attributes.HasFlag(FileAttributes.Hidden))
                    {
                        DirView directoryView = new DirView((discElement as MyDirectory));
                        UserFiles.Items.Add(directoryView);
                    }
                }
            }
        }
    }
}
