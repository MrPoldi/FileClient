using System.Collections.Generic;
using System.Windows;
using Client.Classes;
using Client.Views;
using System.IO;
using System;

namespace Client
{
    public partial class MainWindow : Window
    {
        private readonly FileManager myFileManager;
        private List<DiscElement> myDiscElements;
        private string currentPath;
        private DirectoryInfo dirInfo;
        private MyFile fileToSend;
        private String fileToDownload;
        private ConnectionController myConnection;
        private List<String> SFiles;

        //creating ConnController/FileManager & path selection
        public MainWindow()
        {
            myConnection = new ConnectionController();
            myFileManager = new FileManager();
            InitializeComponent();
            currentPath = "D:\\";
            Refresh(currentPath);
        }

        //this method refresh listbox after directory change
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

        private void ServerFilesRefresh()
        {
            ServerFiles.Items.Clear();

            foreach(String s in SFiles)
            {
                if(s.Contains("."))
                {
                    ServerFile serverFile = new ServerFile(s);
                    serverFile.fileSelection += ServerFile_fileSelection;
                    ServerFiles.Items.Add(serverFile);
                }
            }
        }

        private void ServerFile_fileSelection(string fileName)
        {
            myConnection.FileToDownload = fileName;
            fileToDownload = fileName;
            DownloadB.Content = fileName;
        }

        //FileView and DirectoryView events execution
        private void FileView_fileSelection(MyFile file)
        {
            fileToSend = file;
            myConnection.FileTSName = file.Name;
            myConnection.FileTSPath = file.Path;
            myConnection.FileTSSize = file.Size;
            SendB.Content = "Send: " + fileToSend.Path;
        }

        private void DirectoryView_dirChange(string path)
        {
            Refresh(path);
        }

        //return to directory parent unless you are on the disk
        private void BackB_Click(object sender, RoutedEventArgs e)
        {
            if (currentPath.Length > 3)
            {
                Refresh(dirInfo.Parent.FullName);
            }
        }

        //using ConnectionController to check port/IP address if IP and port box are NOT NULL
        private void GetListOfFiles(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(ServerIPBox.Text) && !string.IsNullOrEmpty(ServerPortBox.Text))
            {
                if (!myConnection.IsConnected)
                {
                    if(myConnection.CheckIP(ServerIPBox.Text, int.Parse(ServerPortBox.Text)))
                    {
                        myConnection.Connect();

                        if (myConnection.IsConnected)
                        {
                            ServerName.Text = "Files from: " + ServerIPBox.Text + ":" + ServerPortBox.Text;
                            ServerName.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 255, 0));
                            SFiles = myConnection.ReturnServerFiles();
                            ServerFilesRefresh();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("You are already connected");
                }

            }
            else
                MessageBox.Show("Enter server IP & port");
        }

        //these methods are not implemented yet. They'll work only if GetListOfFiles() method connect with serv.
        private async void SendFile(object sender, RoutedEventArgs e)
        {
            //if not connected
            if (!myConnection.IsConnected)
                { 
                MessageBox.Show("You are not connected with any server."); 
            }
            //if no file choosen in fileToSend var
            else if(fileToSend == null)
            { 
                MessageBox.Show("No file to send."); 
            }
            else if (myConnection.IsBusy)
            {
                MessageBox.Show("Can't send file if server is busy.");
            }
            else
            {
                ServerStatus(true);
                await myConnection.SendFile();
                ServerStatus(false);
                SFiles = myConnection.ReturnServerFiles();
                ServerFilesRefresh();
            }
        }

        private async void DownloadFile(object sender, RoutedEventArgs e)
        {
            //if not connected
            if (!myConnection.IsConnected)
            { 
                MessageBox.Show("You are not connected with any server."); 
            }
            else if (fileToDownload == null)
            { 
                MessageBox.Show("No file to download.");
            }
            else if (myConnection.IsBusy)
            {
                MessageBox.Show("Can't download file if server is busy.");
            }
            else
            {
                ServerStatus(true);
                await myConnection.GetFile();
                ServerStatus(false);
            }

        }

        private void Disconnect(object sender, RoutedEventArgs e)
        {
            if(myConnection.Disconnect())
            {
                ServerName.Text = "Not connected";
                ServerName.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255,0,0));
                ServerFiles.Items.Clear();
                fileToSend = null;
                fileToDownload = null;
                SendB.Content = "Send file";
                DownloadB.Content = "Download file";
                StatusBox.Text = "Server status : ";
                StatusBox.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));

            }
        }

        private void RefreshServerFiles(object sender, RoutedEventArgs e)
        {
            if (myConnection.IsConnected)
            {
                if(!myConnection.IsBusy)
                {
                    SFiles = myConnection.ReturnServerFiles();
                    ServerFilesRefresh();
                }
                else
                    MessageBox.Show("Server is busy.");
            }
            else
                MessageBox.Show("You are not connected with any server");
        }

        private void ServerStatus(bool isBusy)
        {
            if(isBusy)
            {
                StatusBox.Text = "Server status : Busy";
                StatusBox.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 0));
            }
            else
            {
                StatusBox.Text = "Server status : Free";
                StatusBox.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 255, 0));
            }
        }
    }
}
