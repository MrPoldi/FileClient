using System.Collections.Generic;
using System.Windows;
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
        private MyFile fileToSend;
        private ConnectionController myConnection;

        public MainWindow()
        {
            myConnection = new ConnectionController();
            myFileManager = new FileManager();
            InitializeComponent();
            currentPath = "D:\\";
            Refresh(currentPath);
        }

        private void Refresh(string path)
        {
            UserFiles.Items.Clear();
            PathBox.Text = currentPath = path;
            dirInfo = new DirectoryInfo(path);
            myDiscElements = myFileManager.EnumerateElements(path);

            foreach (DiscElement discElement in myDiscElements)
            {
                if (discElement is MyFile)
                {
                    FileView fileView = new FileView((discElement as MyFile));
                    fileView.fileSelection += FileView_fileSelection;
                    UserFiles.Items.Add(fileView);
                }

                else if (discElement is MyDirectory)
                {
                    DirectoryInfo dir = new DirectoryInfo(discElement.Path);
                    if (!dir.Attributes.HasFlag(FileAttributes.Hidden))
                    {
                        DirView directoryView = new DirView((discElement as MyDirectory));
                        directoryView.dirChange += DirectoryView_dirChange;
                        UserFiles.Items.Add(directoryView);
                    }
                }
            }
        }

        private void FileView_fileSelection(MyFile file)
        {
            fileToSend = file;
            SendB.Content = "Send: " + fileToSend.Path;
        }

        private void DirectoryView_dirChange(string path)
        {
            Refresh(path);
        }

        private void BackB_Click(object sender, RoutedEventArgs e)
        {
            if (currentPath.Length > 3)
            {
                Refresh(dirInfo.Parent.FullName);
            }
        }

        private void GetListOfFiles(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(ServerIPBox.Text) && !string.IsNullOrEmpty(ServerPortBox.Text))
            {
                if (myConnection.CheckIP(ServerIPBox.Text, int.Parse(ServerPortBox.Text)))
                {
                    myConnection.GetListOfFiles();
                }

                if (myConnection.IsConnected)
                { 
                    ServerName.Text = "Files from: " + ServerIPBox.Text + ":" + ServerPortBox.Text; 
                }
                else
                { 
                    ServerName.Text = "Not connected"; 
                }
            }
            else
                MessageBox.Show("Enter server IP & port");
        }

        private void SendFile(object sender, RoutedEventArgs e)
        {
            if (!myConnection.IsConnected)
                { MessageBox.Show("You are not connected with any server."); }
            else
                myConnection.SendFile();
        }

        private void DownloadFile(object sender, RoutedEventArgs e)
        {
            if(!myConnection.IsConnected)
                { MessageBox.Show("You are not connected with any server."); }
            else
                myConnection.GetFile();

        }
    }
}
