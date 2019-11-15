using System;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Text;
using System.IO;
using Client.Classes;
using System.Collections.Generic;

namespace Client.Classes
{
    public class ConnectionController
    {
        private byte[] buffer;
        private IPAddress ipAddr;
        private IPEndPoint endPoint;
        private Socket sender;
        private FileInfo fInfo;
        private long fileToSendSize;
        private string fileTSPath;
        private string fileTSName;
        private bool isConnected;
        private string fileToDownload;

        public long FileTSSize
        {
            set { fileToSendSize = value; }
        }

        public string FileTSPath
        {
            set { fileTSPath = value; }
        }

        public string FileTSName
        {
            set { fileTSName = value; }    
        }

        public string FileToDownload
        {
            set { fileToDownload = value; }
        }

        public bool IsConnected
        {
            get { return isConnected; }
        }

        //try to connect a servert (DONE)
        internal void Connect()
        {
            if (!isConnected)
            {
                try
                {
                    sender.Connect(endPoint);
                    isConnected = true;
                }

                catch (SocketException se)
                {
                    isConnected = false;
                    MessageBox.Show("SocketException " + se.ToString());
                }
            }
            else
            {
                MessageBox.Show("You are already connected");
            }
        }

        //send local file to server and receive refreshed list of files (RECEIVING LIST OF FILES IN PROGRESS)
        internal void SendFile()
        {
            try
            {
                SendRequest("a");

                //sending file name
                buffer = Encoding.ASCII.GetBytes(fileTSName);
                sender.Send(buffer);

                //sending file length if server is ready
                if (IsReady())
                {
                    fInfo = new FileInfo(fileTSPath);
                    buffer = BitConverter.GetBytes(fInfo.Length);
                    sender.Send(buffer);
                }           

                //sending file if server is ready
                if (IsReady())
                {
                    sender.SendFile(fileTSPath);
                }

                if (IsReady())
                    MessageBox.Show("File sent");
                else
                {
                    MessageBox.Show("Server didn't save file : " + fileTSName + ". \nYou are disconnected.");
                }

            }

            catch(SocketException se)
            {
                isConnected = false;
                MessageBox.Show("SocketException " + se.ToString());
            }

        }

        //download chosen file from server list IN PROGRESS
        internal void GetFile()
        {
            try
            { 
                SendRequest("b");

                fileToDownload = "cat1.jpg";

                //sending name of wanted file
                sender.Send(Encoding.ASCII.GetBytes(fileToDownload));
                
                //receiving file length
                sender.Receive(buffer);

            }
            catch(SocketException e)
            {
                isConnected = false;
                MessageBox.Show("SocketException " + e.ToString());
            }
        }
        
        //receiving list of files from server
        internal List<MyFile> ReturnServerFiles()
        {
            List<MyFile> ServFiles = new List<MyFile>();
           
            SendRequest("c");

            return ServFiles;
        }

        //This method checks if IP/port are valid
        internal bool CheckIP(string ip, int port)
        {
            try
            {
                ipAddr = IPAddress.Parse(ip);
                endPoint = new IPEndPoint(ipAddr, port);
                sender = new Socket(ipAddr.AddressFamily,
                                    SocketType.Stream,
                                    ProtocolType.Tcp);
                return true;
            }
            catch
            {
                MessageBox.Show("Enter correct port and IP address");
                isConnected = false;
                return false;
            }
        }

        internal bool Disconnect()
        {
            if (isConnected)
            {
                try
                {
                    buffer = Encoding.ASCII.GetBytes("d");
                    sender.Send(buffer);
                    sender.Shutdown(SocketShutdown.Both);
                    isConnected = false;
                    return true;
                }
                catch(SocketException e)
                {
                    MessageBox.Show(e.ToString());
                    isConnected = false;
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private bool IsReady()
        {
            bool isReady;

            sender.Receive(buffer, sizeof(bool), 0);

            if (isReady = BitConverter.ToBoolean(buffer, 0))
                return true;
            else
                return false;
        }

        private void SendRequest(string x)
        {
            if(isConnected)
            {
                //option conversion
                buffer = Encoding.ASCII.GetBytes(x);
                //sending server option
                sender.Send(buffer);
            }
        }
    }
}