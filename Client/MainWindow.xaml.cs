﻿using System.Collections.Generic;
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
                ServerFiles.Items.Add(s);
            }
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
                if (!myConnection.IsConnected && myConnection.CheckIP(ServerIPBox.Text, int.Parse(ServerPortBox.Text)))
                {
                    myConnection.Connect();

                    if(myConnection.IsConnected)
                    {
                        SFiles = myConnection.ReturnServerFiles();
                        ServerFilesRefresh();
                    }
                }
                else
                {
                    MessageBox.Show("You are already connected");
                }

                //if connection wasn't failed, show server IP:port above the ServerFilesBox
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

        //these methods are not implemented yet. They'll work only if GetListOfFiles() method connect with serv.
        private async void SendFile(object sender, RoutedEventArgs e)
        {
            //if not connected
            if (!myConnection.IsConnected)
                { MessageBox.Show("You are not connected with any server."); }
            //if no file choosen in fileToSend var
            else if(fileToSend == null)
                { MessageBox.Show("No file to send."); }
            else
            {
                await myConnection.SendFile();
            }
        }

        private void DownloadFile(object sender, RoutedEventArgs e)
        {
            //if not connected
            if(!myConnection.IsConnected)
                { MessageBox.Show("You are not connected with any server."); }
            else
                myConnection.GetFile();

        }

        private void Disconnect(object sender, RoutedEventArgs e)
        {
            if(myConnection.Disconnect())
            {
                ServerName.Text = "Not connected";
                ServerFiles.Items.Clear();
            }
        }

        private void RefreshServerFiles(object sender, RoutedEventArgs e)
        {

        }
    }
}
